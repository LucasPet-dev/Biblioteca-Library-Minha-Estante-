using MinhaEstante.Application.Tests.Fakes;
using MinhaEstante.Application.UseCases;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Tests.UseCases;

public sealed class OpenBookUseCaseTests
{
    private const int TotalPages = 10;

    [Fact]
    public async Task ExecuteAsync_NoProgress_StartsAtFirstPage()
    {
        var repository = new FakeBookRepository();
        var book = new Book("Sample", BookFormat.Pdf, "path.pdf", TotalPages);
        await repository.AddAsync(book);

        var useCase = new OpenBookUseCase(repository);

        var result = await useCase.ExecuteAsync(book.Id);

        Assert.Equal(1, result.StartPage);
    }

    [Fact]
    public async Task ExecuteAsync_WithProgress_ReturnsSavedPage()
    {
        var repository = new FakeBookRepository();
        var book = new Book("Sample", BookFormat.Pdf, "path.pdf", TotalPages);
        await repository.AddAsync(book);

        var progress = new ReadingProgress(book.Id);
        progress.MoveTo(7, TotalPages);
        await repository.SaveProgressAsync(progress);

        var useCase = new OpenBookUseCase(repository);

        var result = await useCase.ExecuteAsync(book.Id);

        Assert.Equal(7, result.StartPage);
    }

    [Fact]
    public async Task ExecuteAsync_UnknownBook_Throws()
    {
        var useCase = new OpenBookUseCase(new FakeBookRepository());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => useCase.ExecuteAsync(Guid.NewGuid()));
    }
}