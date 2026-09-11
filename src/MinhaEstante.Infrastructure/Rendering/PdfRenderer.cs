using MinhaEstante.Application.Interfaces;
using PDFtoImage;

namespace MinhaEstante.Infrastructure.Rendering;

public sealed class PdfRenderer : IPdfRenderer
{
    private static readonly RenderOptions RenderOptions = new() { Dpi = 150 };

    public int GetPageCount(string filePath)
    {
        using var stream = File.OpenRead(filePath);
        return Conversion.GetPageCount(stream, leaveOpen: false, password: null);
    }

    public Task<Stream> RenderPageToPngAsync(string filePath, int pageNumber, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        return Task.Run<Stream>(() =>
        {
            using var pdf = File.OpenRead(filePath);
            var output = new MemoryStream();
            Conversion.SavePng(output, pdf, new Index(pageNumber - 1), leaveOpen: false, password: null, options: RenderOptions);
            output.Position = 0;
            return output;
        }, ct);
    }
}