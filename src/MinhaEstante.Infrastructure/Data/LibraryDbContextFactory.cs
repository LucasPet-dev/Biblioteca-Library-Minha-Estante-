using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MinhaEstante.Infrastructure.Data;
using MinhaEstante.Infrastructure.Storage;

namespace MinhaEstante.Infrastructure.Data;

public sealed class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var dataDirectory = FileStorage.DefaultDataDirectory;
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseSqlite($"Data Source={Path.Combine(dataDirectory, "MinhaEstante.db")}")
            .Options;

        return new LibraryDbContext(options);
    }
}