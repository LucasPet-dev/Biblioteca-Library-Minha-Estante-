using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Infrastructure;
using MinhaEstante.Infrastructure.Data;
using MinhaEstante.Infrastructure.Storage;
using MinhaEstante.Presentation.Services;
using MinhaEstante.Presentation.ViewModels;
using MinhaEstante.Presentation.Views;

namespace MinhaEstante.Presentation;

public partial class App : Avalonia.Application
{
    private IServiceProvider? _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _services = ConfigureServices().BuildServiceProvider();
            ApplyMigrations(_services);
            ApplyLanguage(_services);

            var themeService = _services.GetRequiredService<ThemeService>();
            _ = themeService.InitializeAsync();

            var viewModel = _services.GetRequiredService<MainWindowViewModel>();

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };

            _ = viewModel.StartAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static ServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddInfrastructure(AppConfig.ReadDataDirectory());
        services.AddPresentation();
        return services;
    }

    private static void ApplyMigrations(IServiceProvider services)
    {
        using var db = services.GetRequiredService<IDbContextFactory<LibraryDbContext>>()
            .CreateDbContext();
        db.Database.Migrate();
    }

    private static void ApplyLanguage(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var settings = scope.ServiceProvider
            .GetRequiredService<IGetSettingsUseCase>()
            .ExecuteAsync()
            .GetAwaiter()
            .GetResult();

        LocalizationService.Apply(settings.Language);
    }
}