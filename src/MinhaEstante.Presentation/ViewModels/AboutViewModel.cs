using CommunityToolkit.Mvvm.ComponentModel;
using MinhaEstante.Presentation.Resources;

namespace MinhaEstante.Presentation.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _version = "1.0.0";

    public string AppName => Strings.LibraryTitle;

    public string VersionDisplay => $"{Strings.Version} {Version}";
}