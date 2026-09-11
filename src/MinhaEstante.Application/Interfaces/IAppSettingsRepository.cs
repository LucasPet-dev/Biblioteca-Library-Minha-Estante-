using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Persistência das configurações globais do aplicativo (linha única).
/// </summary>
public interface IAppSettingsRepository
{
    Task<AppSettings?> GetAsync(CancellationToken ct = default);

    Task SaveAsync(AppSettings settings, CancellationToken ct = default);
}