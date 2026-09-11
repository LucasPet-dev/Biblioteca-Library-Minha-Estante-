using MinhaEstante.Application.Tests.Fakes;
using MinhaEstante.Application.UseCases;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Tests.UseCases;

public sealed class UpdateReadingProgressUseCaseTests
{
    private const int TotalPages = 10;

    private static async Task<(FakeBookRepository repo, Book book)> Arrange()
    {
        var repository = new FakeBookRepository();
        var book = new Book("Sample", BookFormat.Pdf, "path.pdf", TotalPages);
        await repository.AddAsync(book);
        return (repository, book);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesProgress_WhenNoneExists()
    {
        var (repository, book) = await Arrange();
        var useCase = new UpdateReadingProgressUseCase(repository);

        await useCase.ExecuteAsync(book.Id, 4, TotalPages);

        var progress = await repository.GetProgressAsync(book.Id);
        Assert.NotNull(progress);
        Assert.Equal(4, progress!.CurrentPage);
    }

    [Fact]
    public async Task ExecuteAsync_UpdatesExistingProgress()
    {
        var (repository, book) = await Arrange();
        await repository.SaveProgressAsync(new ReadingProgress(book.Id));

        var useCase = new UpdateReadingProgressUseCase(repository);

        await useCase.ExecuteAsync(book.Id, 9, TotalPages);

        var progress = await repository.GetProgressAsync(book.Id);
        Assert.Equal(9, progress!.CurrentPage);
    }

    [Fact]
    public async Task ExecuteAsync_PageOutOfRange_Throws()
    {
        var (repository, book) = await Arrange();
        var useCase = new UpdateReadingProgressUseCase(repository);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => useCase.ExecuteAsync(book.Id, 99, TotalPages));
    }
}