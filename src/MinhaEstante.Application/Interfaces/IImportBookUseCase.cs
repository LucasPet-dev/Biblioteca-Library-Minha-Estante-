using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Importa um arquivo PDF/CBZ para a biblioteca, copiando o arquivo e gerando a capa.
/// </summary>
public interface IImportBookUseCase
{
    Task<Book> ExecuteAsync(string sourceFilePath, CancellationToken ct = default);
}