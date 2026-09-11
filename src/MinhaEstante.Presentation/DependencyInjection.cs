using Microsoft.Extensions.DependencyInjection;
using MinhaEstante.Presentation.Services;
using MinhaEstante.Presentation.ViewModels;

namespace MinhaEstante.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddSingleton<LibraryViewModel>();
        services.AddSingleton<ImportViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<AboutViewModel>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<ThemeService>();

        services.AddTransient<ReaderViewModel>();
        services.AddTransient<BookDetailsViewModel>();

        services.AddSingleton<Func<Guid, ReaderViewModel>>(sp => bookId =>
        {
            var vm = sp.GetRequiredService<ReaderViewModel>();
            vm.SetBookId(bookId);
            return vm;
        });

        services.AddSingleton<Func<Guid, BookDetailsViewModel>>(sp => bookId =>
        {
            var vm = sp.GetRequiredService<BookDetailsViewModel>();
            vm.SetBookId(bookId);
            return vm;
        });

        return services;
    }
}