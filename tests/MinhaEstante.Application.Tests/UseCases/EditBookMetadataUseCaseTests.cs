using MinhaEstante.Application.Tests.Fakes;
using MinhaEstante.Application.UseCases;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Tests.UseCases;

public sealed class EditBookMetadataUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_UpdatesMetadata()
    {
        var repository = new FakeBookRepository();
        var book = new Book("Old", BookFormat.Pdf, "path.pdf", 10);
        await repository.AddAsync(book);

        var useCase = new EditBookMetadataUseCase(repository);

        await useCase.ExecuteAsync(book.Id, "New Title", "Author", "Mangá");

        var updated = await repository.GetByIdAsync(book.Id);
        Assert.Equal("New Title", updated!.Title);
        Assert.Equal("Author", updated.Author);
        Assert.Equal("Mangá", updated.Genre);
    }

    [Fact]
    public async Task ExecuteAsync_UnknownBook_Throws()
    {
        var useCase = new EditBookMetadataUseCase(new FakeBookRepository());

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => useCase.ExecuteAsync(Guid.NewGuid(), "x", null, null));
    }
}