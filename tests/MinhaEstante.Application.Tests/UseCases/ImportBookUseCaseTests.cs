using MinhaEstante.Application.Tests.Fakes;
using MinhaEstante.Application.UseCases;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Tests.UseCases;

public sealed class ImportBookUseCaseTests
{
    private static string CreateTempFile(string extension)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}{extension}");
        File.WriteAllText(path, "content");
        return path;
    }

    [Fact]
    public async Task ExecuteAsync_ImportsPdf_AddsBookWithCorrectMetadata()
    {
        var source = CreateTempFile(".pdf");
        try
        {
            var repository = new FakeBookRepository();
            var storage = new FakeFileStorage();
            var handler = new FakeFormatHandler(BookFormat.Pdf, ".pdf");
            var useCase = new ImportBookUseCase(new[] { handler }, repository, storage);

            var book = await useCase.ExecuteAsync(source);

            Assert.Equal(BookFormat.Pdf, book.Format);
            Assert.Equal(Path.GetFileNameWithoutExtension(source), book.Title);
            Assert.Equal(handler.TotalPages, book.TotalPages);
            Assert.NotNull(book.CoverImagePath);
            Assert.EndsWith("cover.png", book.CoverImagePath);

            Assert.Single(storage.CopiedFiles);
            Assert.NotNull(await repository.GetByIdAsync(book.Id));
        }
        finally
        {
            File.Delete(source);
        }
    }

    [Fact]
    public async Task ExecuteAsync_UnknownExtension_ThrowsNotSupported()
    {
        var source = CreateTempFile(".epub");
        try
        {
            var useCase = new ImportBookUseCase(
                new[] { new FakeFormatHandler(BookFormat.Pdf, ".pdf") },
                new FakeBookRepository(),
                new FakeFileStorage());

            await Assert.ThrowsAsync<NotSupportedException>(() => useCase.ExecuteAsync(source));
        }
        finally
        {
            File.Delete(source);
        }
    }

    [Fact]
    public async Task ExecuteAsync_MissingFile_ThrowsFileNotFound()
    {
        var useCase = new ImportBookUseCase(
            new[] { new FakeFormatHandler(BookFormat.Pdf, ".pdf") },
            new FakeBookRepository(),
            new FakeFileStorage());

        await Assert.ThrowsAsync<FileNotFoundException>(() => useCase.ExecuteAsync("definitivamente-nao-existe.pdf"));
    }
}