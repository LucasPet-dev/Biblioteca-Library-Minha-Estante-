using Microsoft.EntityFrameworkCore;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Infrastructure.Data;

namespace MinhaEstante.Infrastructure.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly IDbContextFactory<LibraryDbContext> _factory;

    public BookRepository(IDbContextFactory<LibraryDbContext> factory)
    {
        _factory = factory;
    }

    // Cada operação cria e descarta o próprio contexto, pois este repositório pode ser
    // invocado por mais de uma thread/operação simultânea (ViewModels singleton).
    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.Books.FirstOrDefaultAsync(b => b.Id == id, ct);
    }

    public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var books = await db.Books.ToListAsync(ct);
        return books.OrderByDescending(b => b.AddedAt).ToList();
    }

    public async Task AddAsync(Book book, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Books.Add(book);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Book book, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        db.Books.Update(book);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Book book, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var progress = await db.ReadingProgress.FirstOrDefaultAsync(p => p.BookId == book.Id, ct);
        if (progress is not null)
        {
            db.ReadingProgress.Remove(progress);
        }

        db.Books.Remove(book);
        await db.SaveChangesAsync(ct);
    }

    public async Task<ReadingProgress?> GetProgressAsync(Guid bookId, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        return await db.ReadingProgress.FirstOrDefaultAsync(p => p.BookId == bookId, ct);
    }

    public async Task SaveProgressAsync(ReadingProgress progress, CancellationToken ct = default)
    {
        await using var db = _factory.CreateDbContext();
        var existing = await db.ReadingProgress.FirstOrDefaultAsync(p => p.BookId == progress.BookId, ct);

        if (existing is null)
        {
            db.ReadingProgress.Add(progress);
        }
        else
        {
            db.Entry(existing).CurrentValues.SetValues(progress);
        }

        await db.SaveChangesAsync(ct);
    }
}