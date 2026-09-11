using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Infrastructure.Rendering;

namespace MinhaEstante.Infrastructure.Formats;

public sealed class PdfFormatHandler : IBookFormatHandler
{
    private readonly IPdfRenderer _renderer;

    public PdfFormatHandler(IPdfRenderer renderer)
    {
        _renderer = renderer;
    }

    public BookFormat Format => BookFormat.Pdf;

    public bool CanHandle(string filePath) => string.Equals(
        Path.GetExtension(filePath), ".pdf", StringComparison.OrdinalIgnoreCase);

    public Task<int> GetTotalPagesAsync(string filePath, CancellationToken ct = default) =>
        Task.FromResult(_renderer.GetPageCount(filePath));

    public Task<Stream> ExtractCoverAsync(string filePath, CancellationToken ct = default) =>
        _renderer.RenderPageToPngAsync(filePath, 1, ct);

    public Task<Stream> RenderPageAsync(string filePath, int pageNumber, CancellationToken ct = default) =>
        _renderer.RenderPageToPngAsync(filePath, pageNumber, ct);
}