using System.IO.Compression;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Infrastructure.Formats;

public sealed class CbzFormatHandler : IBookFormatHandler
{
    private static readonly string[] ImageExtensions =
    {
        ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp",
    };

    public BookFormat Format => BookFormat.Cbz;

    public bool CanHandle(string filePath) => string.Equals(
        Path.GetExtension(filePath), ".cbz", StringComparison.OrdinalIgnoreCase);

    public Task<int> GetTotalPagesAsync(string filePath, CancellationToken ct = default)
    {
        using var archive = ZipFile.OpenRead(filePath);
        return Task.FromResult(GetImageEntries(archive).Count);
    }

    public Task<Stream> ExtractCoverAsync(string filePath, CancellationToken ct = default) =>
        RenderPageAsync(filePath, 1, ct);

    public Task<Stream> RenderPageAsync(string filePath, int pageNumber, CancellationToken ct = default)
    {
        using var archive = ZipFile.OpenRead(filePath);
        var entries = GetImageEntries(archive);

        if (pageNumber < 1 || pageNumber > entries.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber), $"Page must be between 1 and {entries.Count}.");
        }

        var entry = entries[pageNumber - 1];

        using var entryStream = entry.Open();
        var output = new MemoryStream();
        entryStream.CopyTo(output);
        output.Position = 0;

        return Task.FromResult<Stream>(output);
    }

    private static List<ZipArchiveEntry> GetImageEntries(ZipArchive archive) =>
        archive.Entries
            .Where(e => ImageExtensions.Contains(Path.GetExtension(e.Name), StringComparer.OrdinalIgnoreCase))
            .OrderBy(e => e.FullName, StringComparer.OrdinalIgnoreCase)
            .ToList();
}