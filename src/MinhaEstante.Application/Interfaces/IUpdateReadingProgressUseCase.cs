namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Persiste a página atual da leitura de um livro (e o timestamp de acesso).
/// </summary>
public interface IUpdateReadingProgressUseCase
{
    Task ExecuteAsync(Guid bookId, int page, int totalPages, CancellationToken ct = default);
}