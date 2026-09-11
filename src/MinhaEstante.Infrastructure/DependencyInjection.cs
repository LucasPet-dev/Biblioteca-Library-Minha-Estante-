using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Application.UseCases;
using MinhaEstante.Infrastructure.Data;
using MinhaEstante.Infrastructure.Formats;
using MinhaEstante.Infrastructure.Rendering;
using MinhaEstante.Infrastructure.Repositories;
using MinhaEstante.Infrastructure.Storage;

namespace MinhaEstante.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? dataDirectory = null)
    {
        dataDirectory ??= FileStorage.DefaultDataDirectory;

        services.AddSingleton<IFileStorage>(new FileStorage(dataDirectory));
        services.AddSingleton<IDataDirectoryManager, DataDirectoryManager>();
        services.AddSingleton<IPdfRenderer, PdfRenderer>();

        services.AddSingleton<IBookFormatHandler, PdfFormatHandler>();
        services.AddSingleton<IBookFormatHandler, CbzFormatHandler>();

        // O DbContext é consumido por ViewModels singleton (longa duração) na camada de
// apresentação. Usar AddDbContextFactory garante um contexto novo por operação,
// evitando o compartilhamento de um único DbContext (não thread-safe) entre
// operações concorrentes, o que causava exceções em execução.
        services.AddDbContextFactory<LibraryDbContext>(options =>
            options.UseSqlite($"Data Source={Path.Combine(dataDirectory, "MinhaEstante.db")}"));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IAppSettingsRepository, AppSettingsRepository>();

        services.AddScoped<IImportBookUseCase, ImportBookUseCase>();
        services.AddScoped<IGetLibraryUseCase, GetLibraryUseCase>();
        services.AddScoped<IOpenBookUseCase, OpenBookUseCase>();
        services.AddScoped<IUpdateReadingProgressUseCase, UpdateReadingProgressUseCase>();
        services.AddScoped<IRemoveBookUseCase, RemoveBookUseCase>();
        services.AddScoped<IEditBookMetadataUseCase, EditBookMetadataUseCase>();

        services.AddScoped<IGetSettingsUseCase, GetSettingsUseCase>();
        services.AddScoped<IUpdateSettingsUseCase, UpdateSettingsUseCase>();

        return services;
    }
}