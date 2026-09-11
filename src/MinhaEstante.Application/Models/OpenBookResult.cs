using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.Models;

public sealed record OpenBookResult(Book Book, int StartPage);