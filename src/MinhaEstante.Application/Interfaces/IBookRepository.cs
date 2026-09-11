using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Persistência de livros e do progresso de leitura.
/// </summary>
public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(Book book, CancellationToken ct = default);

    Task UpdateAsync(Book book, CancellationToken ct = default);

    Task DeleteAsync(Book book, CancellationToken ct = default);

    Task<ReadingProgress?> GetProgressAsync(Guid bookId, CancellationToken ct = default);

    Task SaveProgressAsync(ReadingProgress progress, CancellationToken ct = default);
}