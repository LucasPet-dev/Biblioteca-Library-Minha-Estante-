using Avalonia;
using Avalonia.Styling;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Presentation.Services;

public sealed class ThemeService
{
    private readonly IGetSettingsUseCase _getSettings;

    public ThemeService(IGetSettingsUseCase getSettings)
    {
        _getSettings = getSettings;
    }

    public async Task InitializeAsync()
    {
        var settings = await _getSettings.ExecuteAsync();
        ApplyTheme(settings.Theme);
        ApplyCoverSize(settings.CoverSize);
    }

    public void ApplyTheme(AppTheme theme)
    {
        if (Avalonia.Application.Current is null)
        {
            return;
        }

        Avalonia.Application.Current.RequestedThemeVariant =
            theme == AppTheme.Light ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    public void ApplyCoverSize(CoverSize size)
    {
        if (Avalonia.Application.Current is null)
        {
            return;
        }

        var width = size switch
        {
            CoverSize.Small => 160.0,
            CoverSize.Large => 240.0,
            _ => 200.0,
        };

        Avalonia.Application.Current.Resources["BookCardWidth"] = width;
        Avalonia.Application.Current.Resources["CoverHeight"] = width;
    }
}