using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;

namespace MinhaEstante.Application.UseCases;

public sealed class GetSettingsUseCase : IGetSettingsUseCase
{
    private readonly IAppSettingsRepository _repository;

    public GetSettingsUseCase(IAppSettingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppSettings> ExecuteAsync(CancellationToken ct = default) =>
        await _repository.GetAsync(ct) ?? AppSettings.CreateDefault();
}