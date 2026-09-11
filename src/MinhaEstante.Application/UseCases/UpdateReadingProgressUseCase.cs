using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.UseCases;

public sealed class UpdateReadingProgressUseCase : IUpdateReadingProgressUseCase
{
    private readonly IBookRepository _bookRepository;

    public UpdateReadingProgressUseCase(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task ExecuteAsync(Guid bookId, int page, int totalPages, CancellationToken ct = default)
    {
        if (bookId == Guid.Empty)
        {
            throw new ArgumentException("BookId must not be empty.", nameof(bookId));
        }

        if (totalPages < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(totalPages), "TotalPages must be at least 1.");
        }

        var book = await _bookRepository.GetByIdAsync(bookId, ct)
            ?? throw new InvalidOperationException($"Book '{bookId}' was not found.");

        var progress = await _bookRepository.GetProgressAsync(bookId, ct) ?? new ReadingProgress(bookId);

        progress.MoveTo(page, totalPages);

        await _bookRepository.SaveProgressAsync(progress, ct);
    }
}