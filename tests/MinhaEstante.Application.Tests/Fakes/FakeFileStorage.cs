using MinhaEstante.Application.Interfaces;

namespace MinhaEstante.Application.Tests.Fakes;

public sealed class FakeFileStorage : IFileStorage
{
    private readonly string _root;

    public List<string> CopiedFiles { get; } = new();
    public List<Guid> DeletedDirectories { get; } = new();

    public FakeFileStorage()
    {
        _root = Path.Combine(Path.GetTempPath(), "MinhaEstanteTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_root);
    }

    public string DataDirectory => _root;

    public string GetBookDirectory(Guid bookId) => Path.Combine(_root, "books", bookId.ToString("N"));

    public async Task<string> CopyBookFileAsync(string sourceFilePath, Guid bookId, CancellationToken ct = default)
    {
        var dir = GetBookDirectory(bookId);
        Directory.CreateDirectory(dir);

        var dest = Path.Combine(dir, Path.GetFileName(sourceFilePath));
        await using (var src = File.OpenRead(sourceFilePath))
        await using (var dst = File.Create(dest))
        {
            await src.CopyToAsync(dst, ct);
        }

        CopiedFiles.Add(dest);
        return dest;
    }

    public async Task<string> SaveCoverAsync(Stream coverStream, Guid bookId, CancellationToken ct = default)
    {
        var dir = GetBookDirectory(bookId);
        Directory.CreateDirectory(dir);

        var dest = Path.Combine(dir, "cover.png");
        await using var fs = File.Create(dest);
        await coverStream.CopyToAsync(fs, ct);
        return dest;
    }

    public void DeleteBookDirectory(Guid bookId)
    {
        var dir = GetBookDirectory(bookId);

        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, recursive: true);
        }

        DeletedDirectories.Add(bookId);
    }
}