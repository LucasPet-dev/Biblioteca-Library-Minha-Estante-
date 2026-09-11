namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Operações de arquivo de baixo nível: copiar livro importado, salvar capa e remover diretórios.
/// </summary>
public interface IFileStorage
{
    string DataDirectory { get; }

    string GetBookDirectory(Guid bookId);

    Task<string> CopyBookFileAsync(string sourceFilePath, Guid bookId, CancellationToken ct = default);

    Task<string> SaveCoverAsync(Stream coverStream, Guid bookId, CancellationToken ct = default);

    void DeleteBookDirectory(Guid bookId);
}