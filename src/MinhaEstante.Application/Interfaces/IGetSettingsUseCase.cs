using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Lê as configurações atuais, criando os padrões quando ainda não existem.
/// </summary>
public interface IGetSettingsUseCase
{
    Task<AppSettings> ExecuteAsync(CancellationToken ct = default);
}