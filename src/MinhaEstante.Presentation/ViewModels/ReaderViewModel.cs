using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Presentation.Messages;

namespace MinhaEstante.Presentation.ViewModels;

public partial class ReaderViewModel : ViewModelBase
{
    // Limite de páginas mantidas em memória (cache LRU) e raio de pré-carregamento.
    // Evita consumo excessivo de memória ao ler documentos grandes, mantendo as
    // páginas adjacentes prontas para uma navegação fluida.
    private const int CacheLimit = 12;
    private const int PrefetchRadius = 2;

    private readonly IOpenBookUseCase _openBook;
    private readonly IUpdateReadingProgressUseCase _updateProgress;
    private readonly IGetSettingsUseCase _getSettings;
    private readonly IEnumerable<IBookFormatHandler> _handlers;

    private Guid _bookId;
    private Book? _book;
    private IBookFormatHandler? _handler;

    private readonly Dictionary<int, Bitmap> _pageCache = new();
    private readonly LinkedList<int> _lruOrder = new();
    private readonly object _cacheLock = new();
    private int _renderVersion;
    private CancellationTokenSource? _prefetchCts;
    private CancellationTokenSource? _saveDebounceCts;
    private int _saveIntervalSeconds;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private Bitmap? _currentImage;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    [ObservableProperty]
    private string _pageLabel = string.Empty;

    [ObservableProperty]
    private double _zoom = 1.0;

    [ObservableProperty]
    private double _imageWidth;

    [ObservableProperty]
    private double _imageHeight;

    [ObservableProperty]
    private SolidColorBrush _readerBackground = new(Color.Parse("#202020"));

    public ReaderFitMode FitMode { get; private set; }

    public ReaderViewModel(
        IOpenBookUseCase openBook,
        IUpdateReadingProgressUseCase updateProgress,
        IGetSettingsUseCase getSettings,
        IEnumerable<IBookFormatHandler> handlers)
    {
        _openBook = openBook;
        _updateProgress = updateProgress;
        _getSettings = getSettings;
        _handlers = handlers;

        WeakReferenceMessenger.Default.Register<ReaderViewModel, ClearReaderCacheMessage>(this, (r, _) => r.ClearCache());
    }

    private void ClearCache()
    {
        lock (_cacheLock)
        {
            foreach (var bitmap in _pageCache.Values)
            {
                bitmap.Dispose();
            }

            _pageCache.Clear();
            _lruOrder.Clear();
        }
    }

    public void SetBookId(Guid bookId) => _bookId = bookId;

    public async Task LoadAsync()
    {
        var result = await _openBook.ExecuteAsync(_bookId);

        _book = result.Book;
        _bookId = result.Book.Id;
        _handler = _handlers.FirstOrDefault(h => h.CanHandle(result.Book.FilePath))
            ?? throw new NotSupportedException($"No format handler supports '{result.Book.FilePath}'.");

        Title = result.Book.Title;
        TotalPages = result.Book.TotalPages;
        CurrentPage = result.StartPage;
        PageLabel = $"{CurrentPage}/{TotalPages}";

        var settings = await _getSettings.ExecuteAsync();
        Zoom = settings.DefaultZoom;
        FitMode = settings.FitMode;
        _saveIntervalSeconds = settings.ProgressSaveIntervalSeconds;

        await RenderCurrentPageAsync();
        StartPrefetch(CurrentPage);
    }

    [RelayCommand]
    private void NextPage() => ChangePage(1);

    [RelayCommand]
    private void PreviousPage() => ChangePage(-1);

    partial void OnZoomChanged(double value) => UpdateImageSize();

    partial void OnCurrentImageChanged(Bitmap? value) => UpdateImageSize();

    private void UpdateImageSize()
    {
        if (CurrentImage is null)
        {
            ImageWidth = 0;
            ImageHeight = 0;
            return;
        }

        ImageWidth = CurrentImage.Size.Width * Zoom;
        ImageHeight = CurrentImage.Size.Height * Zoom;
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        CancelPrefetch();
        _saveDebounceCts?.Cancel();
        _saveDebounceCts = null;
        await SaveProgressAsync();
        WeakReferenceMessenger.Default.Send(new NavigateHomeMessage());
    }

    private void ChangePage(int delta)
    {
        var next = CurrentPage + delta;

        if (next < 1 || next > TotalPages)
        {
            return;
        }

        CurrentPage = next;
        PageLabel = $"{next}/{TotalPages}";

        _ = RenderCurrentPageAsync();
        StartPrefetch(next);
        ScheduleProgressSave();
    }

    private void ScheduleProgressSave()
    {
        _saveDebounceCts?.Cancel();
        _saveDebounceCts = null;

        if (_saveIntervalSeconds <= 0)
        {
            _ = SaveProgressAsync();
            return;
        }

        var cts = new CancellationTokenSource();
        _saveDebounceCts = cts;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_saveIntervalSeconds), cts.Token);
                await SaveProgressAsync();
            }
            catch (OperationCanceledException)
            {
                // Uma navegação mais recente reagendou o salvamento.
            }
        }, cts.Token);
    }

    private async Task RenderCurrentPageAsync()
    {
        var page = CurrentPage;
        var version = ++_renderVersion;

        try
        {
            var bitmap = await EnsurePageAsync(page);

            if (version == _renderVersion)
            {
                CurrentImage = bitmap;
                PageLabel = $"{page}/{TotalPages}";
            }
        }
        catch
        {
            // Falha na renderização; mantém a página atual.
        }
    }

    private async Task<Bitmap> EnsurePageAsync(int page)
    {
        lock (_cacheLock)
        {
            if (_pageCache.TryGetValue(page, out var cached))
            {
                TouchLocked(page);
                return cached;
            }
        }

        var bitmap = await RenderPageCoreAsync(page);

        lock (_cacheLock)
        {
            StoreLocked(page, bitmap);
        }

        return bitmap;
    }

    private Task<Bitmap> RenderPageCoreAsync(int page)
    {
        if (_book is null || _handler is null)
        {
            throw new InvalidOperationException("Reader is not loaded.");
        }

        return Task.Run<Bitmap>(async () =>
        {
            await using var stream = await _handler.RenderPageAsync(_book.FilePath, page);
            return new Bitmap(stream);
        });
    }

    private void StartPrefetch(int center)
    {
        if (_book is null || _handler is null)
        {
            return;
        }

        _prefetchCts?.Cancel();
        _prefetchCts = new CancellationTokenSource();

        var filePath = _book.FilePath;
        var handler = _handler;
        var totalPages = TotalPages;
        var ct = _prefetchCts.Token;

        var order = BuildPrefetchOrder(center, totalPages);

        _ = Task.Run(async () =>
        {
            foreach (var page in order)
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                lock (_cacheLock)
                {
                    if (_pageCache.ContainsKey(page))
                    {
                        continue;
                    }
                }

                try
                {
                    await using var stream = await handler.RenderPageAsync(filePath, page, ct);
                    var bitmap = new Bitmap(stream);

                    lock (_cacheLock)
                    {
                        StoreLocked(page, bitmap);
                    }
                }
                catch
                {
                    // Página não renderizada; seguimos.
                }
            }
        }, ct);
    }

    private static List<int> BuildPrefetchOrder(int center, int totalPages)
    {
        var order = new List<int> { center };

        for (var d = 1; d <= PrefetchRadius; d++)
        {
            var left = center - d;
            var right = center + d;

            if (left >= 1)
            {
                order.Add(left);
            }

            if (right <= totalPages)
            {
                order.Add(right);
            }
        }

        return order;
    }

    private void TouchLocked(int page)
    {
        _lruOrder.Remove(page);
        _lruOrder.AddFirst(page);
    }

    private void StoreLocked(int page, Bitmap bitmap)
    {
        if (_pageCache.ContainsKey(page))
        {
            return;
        }

        _pageCache[page] = bitmap;
        _lruOrder.AddFirst(page);

        while (_lruOrder.Count > CacheLimit)
        {
            var last = _lruOrder.Last!.Value;
            _lruOrder.RemoveLast();

            if (_pageCache.Remove(last, out var old))
            {
                old.Dispose();
            }
        }
    }

    private void CancelPrefetch() => _prefetchCts?.Cancel();

    private async Task SaveProgressAsync()
    {
        if (_book is null)
        {
            return;
        }

        await _updateProgress.ExecuteAsync(_bookId, CurrentPage, TotalPages);

        WeakReferenceMessenger.Default.Send(new ReadingProgressChangedMessage(_bookId, CurrentPage));
    }
}