using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.UseCases;

public sealed class UpdateSettingsUseCase : IUpdateSettingsUseCase
{
    private readonly IAppSettingsRepository _repository;

    public UpdateSettingsUseCase(IAppSettingsRepository repository)
    {
        _repository = repository;
    }

    public Task ExecuteAsync(AppSettings settings, CancellationToken ct = default) =>
        _repository.SaveAsync(settings, ct);
}