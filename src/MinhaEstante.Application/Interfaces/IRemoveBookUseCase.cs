namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Remove um livro da biblioteca e, opcionalmente, o arquivo físico do disco.
/// </summary>
public interface IRemoveBookUseCase
{
    Task ExecuteAsync(Guid bookId, bool deleteFile, CancellationToken ct = default);
}