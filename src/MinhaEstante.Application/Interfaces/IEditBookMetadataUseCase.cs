namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Altera título, autor e gênero de um livro existente.
/// </summary>
public interface IEditBookMetadataUseCase
{
    Task ExecuteAsync(Guid bookId, string title, string? author, string? genre, CancellationToken ct = default);
}