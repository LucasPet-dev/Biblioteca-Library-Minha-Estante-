using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Abstração polimórfica para importar e ler um formato de arquivo de livro (PDF, CBZ).
/// </summary>
/// <remarks>
/// Cada implementação sabe reconhecer o próprio formato (<see cref="CanHandle"/>),
/// informar o total de páginas, extrair a capa e renderizar uma página. Isso permite
/// adicionar novos formatos (CBR, EPUB...) sem alterar as camadas superiores.
/// </remarks>
public interface IBookFormatHandler
{
    BookFormat Format { get; }

    bool CanHandle(string filePath);

    Task<int> GetTotalPagesAsync(string filePath, CancellationToken ct = default);

    Task<Stream> ExtractCoverAsync(string filePath, CancellationToken ct = default);

    Task<Stream> RenderPageAsync(string filePath, int pageNumber, CancellationToken ct = default);
}