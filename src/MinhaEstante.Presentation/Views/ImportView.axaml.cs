using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MinhaEstante.Presentation.Resources;
using MinhaEstante.Presentation.ViewModels;

namespace MinhaEstante.Presentation.Views;

public partial class ImportView : UserControl
{
    public ImportView()
    {
        InitializeComponent();
        BrowseButton.Click += OnBrowseClicked;
    }

    private async void OnBrowseClicked(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null || DataContext is not ImportViewModel vm)
        {
            return;
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Strings.SelectBook,
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType(Strings.DigitalBooks) { Patterns = new[] { "*.pdf", "*.cbz" } },
            },
        });

        if (files.Count == 0)
        {
            return;
        }

        var path = files[0].TryGetLocalPath();

        if (path is not null)
        {
            vm.SetSourcePath(path);
        }
    }
}