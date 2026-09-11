using MinhaEstante.Application.Models;

namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Retorna os livros da biblioteca com o progresso de leitura consolidado.
/// </summary>
public interface IGetLibraryUseCase
{
    Task<IReadOnlyList<LibraryItem>> ExecuteAsync(CancellationToken ct = default);
}