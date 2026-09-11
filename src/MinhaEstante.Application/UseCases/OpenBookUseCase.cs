using MinhaEstante.Application.Interfaces;
using MinhaEstante.Application.Models;

namespace MinhaEstante.Application.UseCases;

public sealed class OpenBookUseCase : IOpenBookUseCase
{
    private readonly IBookRepository _bookRepository;

    public OpenBookUseCase(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<OpenBookResult> ExecuteAsync(Guid bookId, CancellationToken ct = default)
    {
        var book = await _bookRepository.GetByIdAsync(bookId, ct)
            ?? throw new InvalidOperationException($"Book '{bookId}' was not found.");

        var progress = await _bookRepository.GetProgressAsync(bookId, ct);

        return new OpenBookResult(book, progress?.CurrentPage ?? 1);
    }
}