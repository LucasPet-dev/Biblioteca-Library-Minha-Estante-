using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Domain.Entities;

/// <summary>
/// Representa um livro digital importado para a biblioteca.
/// </summary>
/// <remarks>
/// Um <see cref="Book"/> contém os metadados (título, autor, gênero), o formato,
/// a localização do arquivo copiado e a capa, além do total de páginas usado para
/// calcular o progresso de leitura. O arquivo original não é mantido: na importação
/// o conteúdo é copiado para a pasta de dados e o <see cref="FilePath"/> aponta para a cópia.
/// </remarks>
public class Book
{
    public Guid Id { get; private set; }

    public string Title { get; private set; }

    public string? Author { get; private set; }

    public BookFormat Format { get; private set; }

    public string FilePath { get; private set; }

    public string? CoverImagePath { get; private set; }

    public DateTimeOffset AddedAt { get; private set; }

    public int TotalPages { get; private set; }

    public string? Genre { get; private set; }

    // Construtor parameterless usado apenas pelo EF Core para materializar a entidade.
    private Book()
    {
        Title = null!;
        FilePath = null!;
    }

    public Book(
        string title,
        BookFormat format,
        string filePath,
        int totalPages,
        string? author = null,
        string? genre = null,
        string? coverImagePath = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (totalPages < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(totalPages), "TotalPages must be at least 1.");
        }

        Id = Guid.NewGuid();
        Title = title;
        Author = author;
        Format = format;
        FilePath = filePath;
        TotalPages = totalPages;
        Genre = genre;
        CoverImagePath = coverImagePath;
        AddedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateMetadata(string title, string? author, string? genre)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Title = title;
        Author = author;
        Genre = genre;
    }

    public void SetFilePath(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        FilePath = filePath;
    }

    public void SetCoverImagePath(string coverImagePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(coverImagePath);
        CoverImagePath = coverImagePath;
    }
}