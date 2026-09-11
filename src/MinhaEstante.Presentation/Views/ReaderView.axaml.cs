using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Presentation.ViewModels;

namespace MinhaEstante.Presentation.Views;

public partial class ReaderView : UserControl
{
    private const double MinZoom = 0.05;
    private const double MaxZoom = 5.0;

    private bool _dragging;
    private Point _dragStart;
    private Vector _scrollStart;
    private bool _pendingFit = true;

    public ReaderView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;

        Scroller.PointerPressed += OnPointerPressed;
        Scroller.PointerMoved += OnPointerMoved;
        Scroller.PointerReleased += OnPointerReleased;
        Scroller.AddHandler(PointerWheelChangedEvent, OnPointerWheelChanged, RoutingStrategies.Tunnel);
        Scroller.KeyDown += OnKeyDown;
        Scroller.Loaded += OnScrollerLoaded;
    }

    private void OnScrollerLoaded(object? sender, RoutedEventArgs e)
    {
        Scroller.Focus();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not ReaderViewModel vm)
        {
            return;
        }

        var handled = true;

        switch (e.Key)
        {
            case Key.Left:
            case Key.PageUp:
                vm.PreviousPageCommand.Execute(null);
                break;
            case Key.Right:
            case Key.PageDown:
            case Key.Space:
                vm.NextPageCommand.Execute(null);
                break;
            case Key.Escape:
                vm.BackCommand.Execute(null);
                break;
            default:
                handled = false;
                break;
        }

        if (handled)
        {
            e.Handled = true;
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        _pendingFit = true;

        if (DataContext is ReaderViewModel vm)
        {
            vm.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ReaderViewModel.CurrentImage) && _pendingFit)
        {
            Dispatcher.UIThread.Post(FitToViewport, DispatcherPriority.Loaded);
        }
    }

    // First open: fit the whole page to the viewport.
    private void FitToViewport()
    {
        if (DataContext is not ReaderViewModel vm || vm.CurrentImage is null)
        {
            return;
        }

        var viewport = Scroller.Viewport;

        if (viewport.Width < 1 || viewport.Height < 1)
        {
            Dispatcher.UIThread.Post(FitToViewport, DispatcherPriority.Loaded);
            return;
        }

        var size = vm.CurrentImage.Size;
        var fit = vm.FitMode switch
        {
            ReaderFitMode.Width => viewport.Width / size.Width,
            ReaderFitMode.Height => viewport.Height / size.Height,
            _ => Math.Min(viewport.Width / size.Width, viewport.Height / size.Height),
        };

        vm.Zoom = Math.Clamp(fit, MinZoom, MaxZoom);
        Scroller.Offset = Vector.Zero;
        _pendingFit = false;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(Scroller).Properties.IsLeftButtonPressed)
        {
            return;
        }

        _dragging = true;
        _dragStart = e.GetPosition(Scroller);
        _scrollStart = Scroller.Offset;
        e.Pointer.Capture(Scroller);
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_dragging)
        {
            return;
        }

        var current = e.GetPosition(Scroller);
        var delta = current - _dragStart;
        Scroller.Offset = _scrollStart - new Vector(delta.X, delta.Y);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _dragging = false;
        e.Pointer.Capture(null);
    }

    // Wheel zoom anchored at the cursor position.
    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (DataContext is not ReaderViewModel vm)
        {
            return;
        }

        var factor = e.Delta.Y > 0 ? 1.15 : 1 / 1.15;
        var newZoom = Math.Clamp(vm.Zoom * factor, MinZoom, MaxZoom);
        var actualFactor = newZoom / vm.Zoom;

        var pointer = e.GetPosition(Scroller);
        var contentPoint = Scroller.Offset + new Vector(pointer.X, pointer.Y);
        var newOffset = new Vector(
            contentPoint.X * actualFactor - pointer.X,
            contentPoint.Y * actualFactor - pointer.Y);

        vm.Zoom = newZoom;

        // Apply the new offset after the pending layout pass that the zoom triggers.
        Dispatcher.UIThread.Post(() => Scroller.Offset = newOffset, DispatcherPriority.Input);

        e.Handled = true;
    }
}