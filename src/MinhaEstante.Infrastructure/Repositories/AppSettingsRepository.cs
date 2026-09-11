using Microsoft.EntityFrameworkCore;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Infrastructure.Data;

namespace MinhaEstante.Infrastructure.Repositories;

public sealed class AppSettingsRepository : IAppSettingsRepository
{
    private readonly IDbContextFactory<LibraryDbContext> _factory;

    public AppSettingsRepository(IDbContextFactory<LibraryDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<AppSettings?> GetAsync(CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Settings.FirstOrDefaultAsync(s => s.Id == AppSettings.SingletonId, ct);
    }

    public async Task SaveAsync(AppSettings settings, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var existing = await db.Settings.FirstOrDefaultAsync(s => s.Id == settings.Id, ct);

        if (existing is null)
        {
            db.Settings.Add(settings);
        }
        else
        {
            db.Entry(existing).CurrentValues.SetValues(settings);
        }

        await db.SaveChangesAsync(ct);
    }
}