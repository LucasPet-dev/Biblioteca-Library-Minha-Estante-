using MinhaEstante.Application.Interfaces;
using MinhaEstante.Application.Models;

namespace MinhaEstante.Application.UseCases;

public sealed class GetLibraryUseCase : IGetLibraryUseCase
{
    private readonly IBookRepository _bookRepository;

    public GetLibraryUseCase(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IReadOnlyList<LibraryItem>> ExecuteAsync(CancellationToken ct = default)
    {
        var books = await _bookRepository.GetAllAsync(ct);

        var items = new List<LibraryItem>(books.Count);

        foreach (var book in books)
        {
            var progress = await _bookRepository.GetProgressAsync(book.Id, ct);
            var currentPage = progress?.CurrentPage ?? 1;
            var percent = progress?.GetPercentComplete(book.TotalPages) ?? 0;

            items.Add(new LibraryItem(book, currentPage, percent, progress?.LastReadAt));
        }

        return items;
    }
}