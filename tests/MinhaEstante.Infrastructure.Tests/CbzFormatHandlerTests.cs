using System.IO.Compression;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Infrastructure.Formats;

namespace MinhaEstante.Infrastructure.Tests;

public sealed class CbzFormatHandlerTests
{
    private static string CreateCbz(Func<string, byte[]> contentFactory)
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.cbz");

        using (var zip = ZipFile.Open(path, ZipArchiveMode.Create))
        {
            foreach (var name in new[] { "0001.png", "0002.jpg", "notes.txt" })
            {
                var entry = zip.CreateEntry(name);
                using var stream = entry.Open();
                var bytes = contentFactory(name);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        return path;
    }

    [Fact]
    public void CanHandle_MatchesCbzExtension()
    {
        var handler = new CbzFormatHandler();

        Assert.True(handler.CanHandle("file.cbz"));
        Assert.True(handler.CanHandle("FILE.CBZ"));
        Assert.False(handler.CanHandle("file.pdf"));
    }

    [Fact]
    public async Task GetTotalPagesAsync_CountsImagesOnly()
    {
        var path = CreateCbz(_ => new byte[] { 1, 2, 3 });
        try
        {
            var handler = new CbzFormatHandler();

            var count = await handler.GetTotalPagesAsync(path);

            Assert.Equal(2, count);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task RenderPageAsync_ReturnsImageBytes()
    {
        var path = CreateCbz(name => name == "0001.png" ? new byte[] { 10, 20, 30 } : new byte[] { 40, 50 });
        try
        {
            var handler = new CbzFormatHandler();

            await using var first = await handler.RenderPageAsync(path, 1);
            using var firstMs = new MemoryStream();
            await first.CopyToAsync(firstMs);

            Assert.Equal(new byte[] { 10, 20, 30 }, firstMs.ToArray());
        }
        finally
        {
            File.Delete(path);
        }
    }
}