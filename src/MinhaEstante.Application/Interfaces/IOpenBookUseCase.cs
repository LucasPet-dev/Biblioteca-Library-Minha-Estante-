using MinhaEstante.Application.Models;

namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Abre um livro retornando seus dados e a página de retomada da leitura.
/// </summary>
public interface IOpenBookUseCase
{
    Task<OpenBookResult> ExecuteAsync(Guid bookId, CancellationToken ct = default);
}