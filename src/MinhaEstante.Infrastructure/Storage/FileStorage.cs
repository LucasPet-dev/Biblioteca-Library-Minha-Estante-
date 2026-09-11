using System.IO.Compression;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Infrastructure.Storage;

public sealed class FileStorage : IFileStorage
{
    private const string AppFolderName = "MinhaEstante";

    public string DataDirectory { get; }

    public static string DefaultDataDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        AppFolderName);

    public FileStorage()
        : this(DefaultDataDirectory)
    {
    }

    public FileStorage(string dataDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);

        DataDirectory = dataDirectory;
        Directory.CreateDirectory(DataDirectory);
    }

    public string GetBookDirectory(Guid bookId) =>
        Path.Combine(DataDirectory, "books", bookId.ToString("N"));

    public async Task<string> CopyBookFileAsync(string sourceFilePath, Guid bookId, CancellationToken ct = default)
    {
        var directory = GetBookDirectory(bookId);
        Directory.CreateDirectory(directory);

        var destination = Path.Combine(directory, Path.GetFileName(sourceFilePath));

        await using var source = File.OpenRead(sourceFilePath);
        await using var target = File.Create(destination);
        await source.CopyToAsync(target, ct);

        return destination;
    }

    public async Task<string> SaveCoverAsync(Stream coverStream, Guid bookId, CancellationToken ct = default)
    {
        var directory = GetBookDirectory(bookId);
        Directory.CreateDirectory(directory);

        var destination = Path.Combine(directory, "cover.png");

        await using var target = File.Create(destination);
        await coverStream.CopyToAsync(target, ct);

        return destination;
    }

    public void DeleteBookDirectory(Guid bookId)
    {
        var directory = GetBookDirectory(bookId);

        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}