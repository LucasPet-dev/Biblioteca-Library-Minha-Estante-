using MinhaEstante.Application.Interfaces;

namespace MinhaEstante.Infrastructure.Storage;

public sealed class DataDirectoryManager : IDataDirectoryManager
{
    private readonly IFileStorage _fileStorage;

    public DataDirectoryManager(IFileStorage fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public string CurrentDirectory => _fileStorage.DataDirectory;

    public async Task MoveAsync(string newDirectory, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newDirectory);

        var source = Path.GetFullPath(_fileStorage.DataDirectory)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var destination = Path.GetFullPath(newDirectory)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (string.Equals(source, destination, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (destination.StartsWith(source + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("A pasta de destino não pode estar dentro da pasta atual.");
        }

        Directory.CreateDirectory(destination);
        await CopyDirectoryAsync(source, destination, ct);

        AppConfig.WriteDataDirectory(newDirectory);
    }

    private static async Task CopyDirectoryAsync(string source, string destination, CancellationToken ct)
    {
        foreach (var file in Directory.GetFiles(source))
        {
            ct.ThrowIfCancellationRequested();

            var target = Path.Combine(destination, Path.GetFileName(file));
            File.Copy(file, target, overwrite: true);
            await Task.Yield();
        }

        foreach (var directory in Directory.GetDirectories(source))
        {
            ct.ThrowIfCancellationRequested();

            var target = Path.Combine(destination, Path.GetFileName(directory));
            Directory.CreateDirectory(target);
            await CopyDirectoryAsync(directory, target, ct);
        }
    }
}