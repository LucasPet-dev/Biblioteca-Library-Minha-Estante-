using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Tests.Fakes;

public sealed class FakeFormatHandler : IBookFormatHandler
{
    private readonly string _extension;

    public BookFormat Format { get; }

    public int TotalPages { get; set; } = 10;

    public FakeFormatHandler(BookFormat format, string extension)
    {
        Format = format;
        _extension = extension;
    }

    public bool CanHandle(string filePath) =>
        string.Equals(Path.GetExtension(filePath), _extension, StringComparison.OrdinalIgnoreCase);

    public Task<int> GetTotalPagesAsync(string filePath, CancellationToken ct = default) =>
        Task.FromResult(TotalPages);

    public Task<Stream> ExtractCoverAsync(string filePath, CancellationToken ct = default) =>
        Task.FromResult<Stream>(new MemoryStream(new byte[] { 1, 2, 3, 4 }));

    public Task<Stream> RenderPageAsync(string filePath, int pageNumber, CancellationToken ct = default) =>
        Task.FromResult<Stream>(new MemoryStream(new byte[] { 5, 6, 7, 8 }));
}