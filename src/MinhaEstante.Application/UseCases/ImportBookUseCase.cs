using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.UseCases;

public sealed class ImportBookUseCase : IImportBookUseCase
{
    private readonly IEnumerable<IBookFormatHandler> _formatHandlers;
    private readonly IBookRepository _bookRepository;
    private readonly IFileStorage _fileStorage;

    public ImportBookUseCase(
        IEnumerable<IBookFormatHandler> formatHandlers,
        IBookRepository bookRepository,
        IFileStorage fileStorage)
    {
        _formatHandlers = formatHandlers;
        _bookRepository = bookRepository;
        _fileStorage = fileStorage;
    }

    public async Task<Book> ExecuteAsync(string sourceFilePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath))
        {
            throw new ArgumentException("Source file path must not be empty.", nameof(sourceFilePath));
        }

        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException("Source file was not found.", sourceFilePath);
        }

        var handler = _formatHandlers.FirstOrDefault(h => h.CanHandle(sourceFilePath))
            ?? throw new NotSupportedException($"No format handler supports the file '{sourceFilePath}'.");

        var title = Path.GetFileNameWithoutExtension(sourceFilePath);
        var totalPages = await handler.GetTotalPagesAsync(sourceFilePath, ct);
        await using var cover = await handler.ExtractCoverAsync(sourceFilePath, ct);

        var book = new Book(title, handler.Format, sourceFilePath, totalPages);

        // O livro é criado com o caminho de origem apenas para satisfazer o construtor;
        // em seguida o arquivo e a capa são copiados para a pasta de dados e o caminho
        // é sobrescrito para apontar para a cópia persistente.
        var finalPath = await _fileStorage.CopyBookFileAsync(sourceFilePath, book.Id, ct);
        var coverPath = await _fileStorage.SaveCoverAsync(cover, book.Id, ct);

        book.SetFilePath(finalPath);
        book.SetCoverImagePath(coverPath);

        await _bookRepository.AddAsync(book, ct);

        return book;
    }
}