using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinhaEstante.Presentation.Messages;
using MinhaEstante.Presentation.Resources;

namespace MinhaEstante.Presentation.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly LibraryViewModel _library;
    private readonly ImportViewModel _import;

    public ObservableCollection<NavItemViewModel> NavItems { get; } = new();

    [ObservableProperty]
    private NavItemViewModel? _selectedItem;

    [ObservableProperty]
    private ViewModelBase? _currentPage;

    [ObservableProperty]
    private bool _isLibraryActive = true;

    public MainWindowViewModel(
        LibraryViewModel library,
        SettingsViewModel settings,
        AboutViewModel about,
        ImportViewModel import)
    {
        _library = library;
        _import = import;

        NavItems.Add(new NavItemViewModel(Strings.SectionLibrary, "▦", library, SelectSection));
        NavItems.Add(new NavItemViewModel(Strings.SectionSettings, "⚙", settings, SelectSection));
        NavItems.Add(new NavItemViewModel(Strings.SectionAbout, "ⓘ", about, SelectSection));

        SelectSection(NavItems[0]);

        WeakReferenceMessenger.Default.Register<MainWindowViewModel, NavigateMessage>(this, (r, m) => r.CurrentPage = m.ViewModel);
        WeakReferenceMessenger.Default.Register<MainWindowViewModel, NavigateHomeMessage>(this, (r, m) => r.GoHome());
    }

    [RelayCommand]
    private void Import()
    {
        WeakReferenceMessenger.Default.Send(new NavigateMessage(_import));
    }

    public async Task StartAsync() => await _library.InitializeAsync();

    private void SelectSection(NavItemViewModel item)
    {
        foreach (var nav in NavItems)
        {
            nav.IsActive = nav == item;
        }

        SelectedItem = item;
        CurrentPage = item.Page;
        IsLibraryActive = item.Page == _library;
    }

    private void GoHome()
    {
        SelectSection(NavItems[0]);
        _ = _library.RefreshAsync();
    }
}