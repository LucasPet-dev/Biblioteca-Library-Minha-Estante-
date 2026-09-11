using MinhaEstante.Application.Interfaces;

namespace MinhaEstante.Application.UseCases;

public sealed class EditBookMetadataUseCase : IEditBookMetadataUseCase
{
    private readonly IBookRepository _bookRepository;

    public EditBookMetadataUseCase(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task ExecuteAsync(
        Guid bookId,
        string title,
        string? author,
        string? genre,
        CancellationToken ct = default)
    {
        var book = await _bookRepository.GetByIdAsync(bookId, ct)
            ?? throw new InvalidOperationException($"Book '{bookId}' was not found.");

        book.UpdateMetadata(title, author, genre);

        await _bookRepository.UpdateAsync(book, ct);
    }
}