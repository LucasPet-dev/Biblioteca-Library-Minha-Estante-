using MinhaEstante.Application.Tests.Fakes;
using MinhaEstante.Application.UseCases;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Tests.UseCases;

public sealed class RemoveBookUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_RemovesBookReference_Only()
    {
        var repository = new FakeBookRepository();
        var storage = new FakeFileStorage();
        var book = new Book("Sample", BookFormat.Pdf, "path.pdf", 10);
        await repository.AddAsync(book);

        var useCase = new RemoveBookUseCase(repository, storage);

        await useCase.ExecuteAsync(book.Id, deleteFile: false);

        Assert.Null(await repository.GetByIdAsync(book.Id));
        Assert.Empty(storage.DeletedDirectories);
    }

    [Fact]
    public async Task ExecuteAsync_WithDeleteFile_DeletesDirectory()
    {
        var repository = new FakeBookRepository();
        var storage = new FakeFileStorage();
        var book = new Book("Sample", BookFormat.Pdf, "path.pdf", 10);
        await repository.AddAsync(book);
        Directory.CreateDirectory(storage.GetBookDirectory(book.Id));

        var useCase = new RemoveBookUseCase(repository, storage);

        await useCase.ExecuteAsync(book.Id, deleteFile: true);

        Assert.Null(await repository.GetByIdAsync(book.Id));
        Assert.Contains(book.Id, storage.DeletedDirectories);
    }
}