using MinhaEstante.Application.Interfaces;

namespace MinhaEstante.Application.UseCases;

public sealed class RemoveBookUseCase : IRemoveBookUseCase
{
    private readonly IBookRepository _bookRepository;
    private readonly IFileStorage _fileStorage;

    public RemoveBookUseCase(IBookRepository bookRepository, IFileStorage fileStorage)
    {
        _bookRepository = bookRepository;
        _fileStorage = fileStorage;
    }

    public async Task ExecuteAsync(Guid bookId, bool deleteFile, CancellationToken ct = default)
    {
        var book = await _bookRepository.GetByIdAsync(bookId, ct);

        if (book is null)
        {
            return;
        }

        await _bookRepository.DeleteAsync(book, ct);

        if (deleteFile)
        {
            _fileStorage.DeleteBookDirectory(bookId);
        }
    }
}