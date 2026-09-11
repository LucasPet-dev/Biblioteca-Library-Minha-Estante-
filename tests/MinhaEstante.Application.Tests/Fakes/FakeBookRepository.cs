using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.Tests.Fakes;

public sealed class FakeBookRepository : IBookRepository
{
    private readonly Dictionary<Guid, Book> _books = new();
    private readonly Dictionary<Guid, ReadingProgress> _progress = new();

    public Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        _books.TryGetValue(id, out var book);
        return Task.FromResult(book);
    }

    public Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default)
    {
        return Task.FromResult<IReadOnlyList<Book>>(_books.Values.ToList());
    }

    public Task AddAsync(Book book, CancellationToken ct = default)
    {
        _books[book.Id] = book;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Book book, CancellationToken ct = default)
    {
        _books[book.Id] = book;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Book book, CancellationToken ct = default)
    {
        _books.Remove(book.Id);
        _progress.Remove(book.Id);
        return Task.CompletedTask;
    }

    public Task<ReadingProgress?> GetProgressAsync(Guid bookId, CancellationToken ct = default)
    {
        _progress.TryGetValue(bookId, out var progress);
        return Task.FromResult(progress);
    }

    public Task SaveProgressAsync(ReadingProgress progress, CancellationToken ct = default)
    {
        _progress[progress.BookId] = progress;
        return Task.CompletedTask;
    }
}