using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MinhaEstante.Presentation.ViewModels;

public partial class NavItemViewModel : ViewModelBase
{
    public string Label { get; }

    public string Glyph { get; }

    public ViewModelBase Page { get; }

    public IRelayCommand SelectCommand { get; }

    [ObservableProperty]
    private bool _isActive;

    public NavItemViewModel(string label, string glyph, ViewModelBase page, Action<NavItemViewModel> onSelect)
    {
        Label = label;
        Glyph = glyph;
        Page = page;
        SelectCommand = new RelayCommand(() => onSelect(this));
    }
}