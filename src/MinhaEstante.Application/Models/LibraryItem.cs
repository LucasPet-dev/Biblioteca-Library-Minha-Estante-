using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.Models;

public sealed record LibraryItem(Book Book, int CurrentPage, double PercentComplete, DateTimeOffset? LastReadAt);