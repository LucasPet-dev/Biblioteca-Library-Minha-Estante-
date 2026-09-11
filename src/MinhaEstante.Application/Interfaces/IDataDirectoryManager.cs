namespace MinhaEstante.Application.Interfaces;

/// <summary>
/// Gerencia a localização da pasta de dados (onde os livros e o banco são armazenados).
/// </summary>
public interface IDataDirectoryManager
{
    string CurrentDirectory { get; }

    Task MoveAsync(string newDirectory, CancellationToken ct = default);
}