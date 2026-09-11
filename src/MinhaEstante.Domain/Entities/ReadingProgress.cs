namespace MinhaEstante.Domain.Entities;

/// <summary>
/// Persiste a posição de leitura de um livro (página atual e última data de acesso).
/// </summary>
/// <remarks>
/// A relação com <see cref="Book"/> é 1:1 pelo <see cref="BookId"/> (chave primária).
/// O <see cref="LastReadAt"/> é atualizado a cada avanço de página e usado como base
/// da ordenação "Último acessado" na biblioteca.
/// </remarks>
public class ReadingProgress
{
    public Guid BookId { get; private set; }

    public int CurrentPage { get; private set; }

    public DateTimeOffset LastReadAt { get; private set; }

    // Construtor parameterless usado apenas pelo EF Core.
    private ReadingProgress()
    {
    }

    public ReadingProgress(Guid bookId)
    {
        if (bookId == Guid.Empty)
        {
            throw new ArgumentException("BookId must not be empty.", nameof(bookId));
        }

        BookId = bookId;
        CurrentPage = 1;
        LastReadAt = DateTimeOffset.UtcNow;
    }

    public void MoveTo(int page, int totalPages)
    {
        if (page < 1 || (totalPages > 0 && page > totalPages))
        {
            throw new ArgumentOutOfRangeException(nameof(page), $"Page must be between 1 and {totalPages}.");
        }

        CurrentPage = page;
        LastReadAt = DateTimeOffset.UtcNow;
    }

    public double GetPercentComplete(int totalPages)
    {
        if (totalPages < 1)
        {
            return 0;
        }

        return Math.Clamp((double)CurrentPage / totalPages * 100, 0, 100);
    }
}