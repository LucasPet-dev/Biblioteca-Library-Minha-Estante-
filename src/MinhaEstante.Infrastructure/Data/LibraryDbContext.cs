using Microsoft.EntityFrameworkCore;
using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Infrastructure.Data;

public sealed class LibraryDbContext : DbContext
{
    public DbSet<Book> Books => Set<Book>();

    public DbSet<ReadingProgress> ReadingProgress => Set<ReadingProgress>();

    public DbSet<AppSettings> Settings => Set<AppSettings>();

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(builder =>
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Title).IsRequired();
            builder.Property(b => b.FilePath).IsRequired();
            builder.Property(b => b.Format).HasConversion<string>().IsRequired();
            builder.Property(b => b.Author).IsRequired(false);
            builder.Property(b => b.Genre).IsRequired(false);
            builder.Property(b => b.CoverImagePath).IsRequired(false);
        });

        modelBuilder.Entity<ReadingProgress>(builder =>
        {
            builder.HasKey(p => p.BookId);
            builder.Property(p => p.CurrentPage).IsRequired();
            builder.Property(p => p.LastReadAt).IsRequired();
        });

        modelBuilder.Entity<AppSettings>(builder =>
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.FitMode).HasConversion<string>().IsRequired();
            builder.Property(s => s.Theme).HasConversion<string>().IsRequired();
            builder.Property(s => s.CoverSize).HasConversion<string>().IsRequired();
            builder.Property(s => s.SortOrder).HasConversion<string>().IsRequired();
            builder.Property(s => s.Language).HasConversion<string>().IsRequired();
        });
    }
}