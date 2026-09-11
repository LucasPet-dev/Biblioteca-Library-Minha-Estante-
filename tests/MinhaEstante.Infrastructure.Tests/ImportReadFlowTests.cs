using System.IO.Compression;
using Microsoft.EntityFrameworkCore;
using MinhaEstante.Application.UseCases;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Infrastructure.Data;
using MinhaEstante.Infrastructure.Formats;
using MinhaEstante.Infrastructure.Repositories;
using MinhaEstante.Infrastructure.Storage;

namespace MinhaEstante.Infrastructure.Tests;

public sealed class ImportReadFlowTests : IDisposable
{
    private readonly string _dataDirectory;

    public ImportReadFlowTests()
    {
        _dataDirectory = Path.Combine(Path.GetTempPath(), "MinhaEstanteIntegration", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_dataDirectory);
    }

    public void Dispose()
    {
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();

        if (Directory.Exists(_dataDirectory))
        {
            Directory.Delete(_dataDirectory, recursive: true);
        }
    }

    private static string CreateCbz(string directory)
    {
        var path = Path.Combine(directory, "sample-manga.cbz");

        using (var zip = ZipFile.Open(path, ZipArchiveMode.Create))
        {
            for (var i = 1; i <= 5; i++)
            {
                var entry = zip.CreateEntry($"page{i:0000}.png");
                using var stream = entry.Open();
                var bytes = new byte[] { (byte)i, 0, 0, 255 };
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        return path;
    }

    private sealed class TestDbContextFactory : IDbContextFactory<LibraryDbContext>
    {
        private readonly string _connection;

        public TestDbContextFactory(string connection) => _connection = connection;

        public LibraryDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<LibraryDbContext>().UseSqlite(_connection).Options);
    }

    private IDbContextFactory<LibraryDbContext> CreateFactory()
    {
        var connection = $"Data Source={Path.Combine(_dataDirectory, "test.db")}";
        using var context = new LibraryDbContext(
            new DbContextOptionsBuilder<LibraryDbContext>().UseSqlite(connection).Options);
        context.Database.EnsureCreated();
        return new TestDbContextFactory(connection);
    }

    [Fact]
    public async Task ImportThenReadThenResumeProgress()
    {
        var source = CreateCbz(_dataDirectory);
        var repository = new BookRepository(CreateFactory());
        var storage = new FileStorage(Path.Combine(_dataDirectory, "appdata"));
        var handler = new CbzFormatHandler();

        var import = new ImportBookUseCase(new[] { handler }, repository, storage);

        var book = await import.ExecuteAsync(source);

        Assert.Equal(BookFormat.Cbz, book.Format);
        Assert.Equal(5, book.TotalPages);
        Assert.True(File.Exists(book.FilePath));
        Assert.True(File.Exists(book.CoverImagePath!));

        var getLibrary = new GetLibraryUseCase(repository);
        var library = await getLibrary.ExecuteAsync();

        var item = Assert.Single(library);
        Assert.Equal(book.Id, item.Book.Id);
        Assert.Equal(1, item.CurrentPage);

        var openBook = new OpenBookUseCase(repository);
        var opened = await openBook.ExecuteAsync(book.Id);
        Assert.Equal(1, opened.StartPage);

        var updateProgress = new UpdateReadingProgressUseCase(repository);
        await updateProgress.ExecuteAsync(book.Id, 3, book.TotalPages);

        var resumed = await openBook.ExecuteAsync(book.Id);
        Assert.Equal(3, resumed.StartPage);
    }
}