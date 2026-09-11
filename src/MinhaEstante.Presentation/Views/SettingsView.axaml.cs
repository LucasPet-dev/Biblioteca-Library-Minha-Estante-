using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MinhaEstante.Presentation.Resources;
using MinhaEstante.Presentation.ViewModels;

namespace MinhaEstante.Presentation.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        BrowseFolderButton.Click += OnBrowseFolderClicked;
    }

    private async void OnBrowseFolderClicked(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null || DataContext is not SettingsViewModel vm)
        {
            return;
        }

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = Strings.ChooseLibraryFolder,
            AllowMultiple = false,
        });

        if (folders.Count == 0)
        {
            return;
        }

        var path = folders[0].TryGetLocalPath();

        if (path is not null)
        {
            await vm.MoveDataDirectoryAsync(path);
        }
    }
}