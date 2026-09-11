namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Encapsula a conversão PDF → imagem (biblioteca PDFtoImage/PDFium).
/// </summary>
/// <remarks>
/// Isolada em uma interface própria para separar a renderização de PDF do contrato de formato,
/// permitindo trocar a biblioteca de renderização sem afetar o restante do sistema.
/// </remarks>
public interface IPdfRenderer
{
    int GetPageCount(string filePath);

    Task<Stream> RenderPageToPngAsync(string filePath, int pageNumber, CancellationToken ct = default);
}