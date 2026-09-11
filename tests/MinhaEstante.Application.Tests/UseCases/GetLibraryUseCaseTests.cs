using MinhaEstante.Application.Tests.Fakes;
using MinhaEstante.Application.UseCases;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Tests.UseCases;

public sealed class GetLibraryUseCaseTests
{
    private static Book CreateBook(string title) => new(title, BookFormat.Pdf, "path.pdf", 10);

    [Fact]
    public async Task ExecuteAsync_EmptyRepository_ReturnsEmpty()
    {
        var useCase = new GetLibraryUseCase(new FakeBookRepository());

        var result = await useCase.ExecuteAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteAsync_WithProgress_ComputesPercentComplete()
    {
        var repository = new FakeBookRepository();
        var book = CreateBook("Sample");
        await repository.AddAsync(book);

        var progress = new ReadingProgress(book.Id);
        progress.MoveTo(5, 10);
        await repository.SaveProgressAsync(progress);

        var useCase = new GetLibraryUseCase(repository);

        var result = await useCase.ExecuteAsync();

        var item = Assert.Single(result);
        Assert.Equal(book.Id, item.Book.Id);
        Assert.Equal(5, item.CurrentPage);
        Assert.Equal(50, item.PercentComplete);
    }
}