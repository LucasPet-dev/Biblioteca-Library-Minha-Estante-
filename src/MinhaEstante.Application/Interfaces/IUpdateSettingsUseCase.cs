using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Persiste as configurações globais do aplicativo.
/// </summary>
public interface IUpdateSettingsUseCase
{
    Task ExecuteAsync(AppSettings settings, CancellationToken ct = default);
}