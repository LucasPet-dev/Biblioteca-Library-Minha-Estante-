using CommunityToolkit.Mvvm.ComponentModel;

namespace MinhaEstante.Presentation.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _appName = "Minha Estante";

    [ObservableProperty]
    private string _version = "1.0.0";
}