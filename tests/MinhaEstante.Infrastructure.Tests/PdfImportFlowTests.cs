using System.Text;
using Microsoft.EntityFrameworkCore;
using MinhaEstante.Application.UseCases;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Infrastructure.Data;
using MinhaEstante.Infrastructure.Formats;
using MinhaEstante.Infrastructure.Rendering;
using MinhaEstante.Infrastructure.Repositories;
using MinhaEstante.Infrastructure.Storage;

namespace MinhaEstante.Infrastructure.Tests;

public sealed class PdfImportFlowTests : IDisposable
{
    private readonly string _dataDirectory;

    public PdfImportFlowTests()
    {
        _dataDirectory = Path.Combine(Path.GetTempPath(), "MinhaEstantePdf", Guid.NewGuid().ToString("N"));
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

    private static string CreatePdf(string directory, int pageCount)
    {
        var path = Path.Combine(directory, "sample.pdf");
        File.WriteAllBytes(path, BuildMinimalPdf(pageCount));
        return path;
    }

    private static byte[] BuildMinimalPdf(int pageCount)
    {
        var objects = new Dictionary<int, string>
        {
            [1] = "<< /Type /Catalog /Pages 2 0 R >>",
            [2] = $"<< /Type /Pages /Kids [{string.Join(" ", Enumerable.Range(0, pageCount).Select(k => $"{3 + k} 0 R"))}] /Count {pageCount} >>",
        };

        for (var i = 0; i < pageCount; i++)
        {
            objects[3 + i] = "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 200 200] >>";
        }

        var sb = new StringBuilder();
        sb.Append("%PDF-1.4\n");

        var offsets = new Dictionary<int, long>();
        var maxObject = 2 + pageCount;

        for (var n = 1; n <= maxObject; n++)
        {
            offsets[n] = Encoding.ASCII.GetByteCount(sb.ToString());
            sb.Append($"{n} 0 obj\n");
            sb.Append(objects[n]);
            sb.Append("\nendobj\n");
        }

        var xrefOffset = Encoding.ASCII.GetByteCount(sb.ToString());
        sb.Append($"xref\n0 {maxObject + 1}\n");
        sb.Append("0000000000 65535 f \n");

        for (var n = 1; n <= maxObject; n++)
        {
            sb.Append($"{offsets[n]:0000000000} 00000 n \n");
        }

        sb.Append($"trailer\n<< /Size {maxObject + 1} /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF\n");

        return Encoding.ASCII.GetBytes(sb.ToString());
    }

    private static bool LooksLikePng(string path)
    {
        var bytes = File.ReadAllBytes(path);
        return bytes.Length > 8
            && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47;
    }

    private sealed class TestDbContextFactory : IDbContextFactory<LibraryDbContext>
    {
        private readonly string _connection;

        public TestDbContextFactory(string connection) => _connection = connection;

        public LibraryDbContext CreateDbContext() =>
            new(new DbContextOptionsBuilder<LibraryDbContext>().UseSqlite(_connection).Options);
    }

    [Fact]
    public async Task ImportPdf_RendersCoverAndPages_WithRealRenderer()
    {
        var source = CreatePdf(_dataDirectory, pageCount: 3);

        var renderer = new PdfRenderer();
        var handler = new PdfFormatHandler(renderer);

        Assert.Equal(3, await handler.GetTotalPagesAsync(source));

        var connection = $"Data Source={Path.Combine(_dataDirectory, "test.db")}";
        using (var context = new LibraryDbContext(
            new DbContextOptionsBuilder<LibraryDbContext>().UseSqlite(connection).Options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        var repository = new BookRepository(new TestDbContextFactory(connection));
        var storage = new FileStorage(Path.Combine(_dataDirectory, "appdata"));
        var import = new ImportBookUseCase(new[] { handler }, repository, storage);

        var book = await import.ExecuteAsync(source);

        Assert.Equal(BookFormat.Pdf, book.Format);
        Assert.Equal(3, book.TotalPages);
        Assert.True(File.Exists(book.FilePath));
        Assert.True(LooksLikePng(book.CoverImagePath!), "Capa deve ser um PNG valido.");

        await using var page2 = await handler.RenderPageAsync(book.FilePath, 2);
        using var buffer = new MemoryStream();
        await page2.CopyToAsync(buffer);

        Assert.True(buffer.Length > 0, "Renderizacao da pagina deve gerar bytes.");
        Assert.Equal(0x89, buffer.ToArray()[0]);
    }
}