using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MinhaEstante.Application.Interfaces;
using MinhaEstante.Domain.Entities;
using MinhaEstante.Domain.Enums;
using MinhaEstante.Presentation.Messages;
using MinhaEstante.Presentation.Services;

namespace MinhaEstante.Presentation.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IGetSettingsUseCase _getSettings;
    private readonly IUpdateSettingsUseCase _updateSettings;
    private readonly IDataDirectoryManager _dataDirectoryManager;
    private readonly ThemeService _theme;

    private AppSettings? _settings;
    private bool _loading;

    public IReadOnlyList<string> FitModeOptions { get; } =
    [
        "Ajustar à página",
        "Ajustar à largura",
        "Altura total",
    ];

    public IReadOnlyList<string> CoverSizeOptions { get; } =
    [
        "Pequena",
        "Média",
        "Grande",
    ];

    public IReadOnlyList<string> SortOrderOptions { get; } =
    [
        "Título",
        "Autor",
        "Data adicionado",
        "Último acessado",
    ];

    private static readonly int[] _progressSaveIntervals = [0, 5, 10, 30];

    public IReadOnlyList<string> LanguageOptions { get; } =
    [
        "Português",
        "English",
    ];

    public IReadOnlyList<string> ProgressSaveIntervalOptions { get; } =
    [
        "A cada mudança",
        "5 segundos",
        "10 segundos",
        "30 segundos",
    ];

    [ObservableProperty]
    private double _defaultZoom = 1.0;

    [ObservableProperty]
    private int _fitModeIndex;

    [ObservableProperty]
    private bool _isDarkTheme = true;

    [ObservableProperty]
    private int _coverSizeIndex = 1;

    [ObservableProperty]
    private int _sortOrderIndex;

    [ObservableProperty]
    private string _dataDirectory = string.Empty;

    [ObservableProperty]
    private string _storageStatus = string.Empty;

    [ObservableProperty]
    private int _languageIndex;

    [ObservableProperty]
    private int _progressSaveIntervalIndex;

    public SettingsViewModel(
        IGetSettingsUseCase getSettings,
        IUpdateSettingsUseCase updateSettings,
        IDataDirectoryManager dataDirectoryManager,
        ThemeService theme)
    {
        _getSettings = getSettings;
        _updateSettings = updateSettings;
        _dataDirectoryManager = dataDirectoryManager;
        _theme = theme;
        DataDirectory = dataDirectoryManager.CurrentDirectory;
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        _loading = true;

        _settings = await _getSettings.ExecuteAsync();

        DefaultZoom = _settings.DefaultZoom;
        FitModeIndex = (int)_settings.FitMode;

        IsDarkTheme = _settings.Theme == AppTheme.Dark;
        CoverSizeIndex = (int)_settings.CoverSize;

        SortOrderIndex = (int)_settings.SortOrder;

        LanguageIndex = (int)_settings.Language;
        ProgressSaveIntervalIndex = Array.IndexOf(_progressSaveIntervals, _settings.ProgressSaveIntervalSeconds);
        if (ProgressSaveIntervalIndex < 0)
        {
            ProgressSaveIntervalIndex = 0;
        }

        _loading = false;
    }

    partial void OnDefaultZoomChanged(double value) => _ = SaveAsync();

    partial void OnFitModeIndexChanged(int value) => _ = SaveAsync();

    partial void OnIsDarkThemeChanged(bool value) => _ = SaveAsync();

    partial void OnCoverSizeIndexChanged(int value) => _ = SaveAsync();

    partial void OnSortOrderIndexChanged(int value) => _ = SaveAndNotifyLibraryAsync();

    partial void OnLanguageIndexChanged(int value)
    {
        _ = SaveAsync();
        if (!_loading)
        {
            StorageStatus = "O idioma será aplicado ao reiniciar o aplicativo.";
        }
    }

    partial void OnProgressSaveIntervalIndexChanged(int value) => _ = SaveAsync();

    private async Task SaveAndNotifyLibraryAsync()
    {
        await SaveAsync();
        NotifyLibraryChanged();
    }

    private void NotifyLibraryChanged()
    {
        if (_loading)
        {
            return;
        }

        WeakReferenceMessenger.Default.Send(new LibrarySettingsChangedMessage());
    }

    public async Task MoveDataDirectoryAsync(string newDirectory)
    {
        if (string.IsNullOrWhiteSpace(newDirectory))
        {
            return;
        }

        try
        {
            await _dataDirectoryManager.MoveAsync(newDirectory);
            StorageStatus = "Dados copiados. Reinicie o aplicativo para usar a nova pasta.";
        }
        catch (Exception ex)
        {
            StorageStatus = $"Erro ao mover: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ClearCache()
    {
        WeakReferenceMessenger.Default.Send(new ClearReaderCacheMessage());
        StorageStatus = "Cache de páginas limpo.";
    }

    private async Task SaveAsync()
    {
        if (_loading || _settings is null)
        {
            return;
        }

        _settings.UpdateReaderSettings(
            DefaultZoom,
            (ReaderFitMode)FitModeIndex);

        var theme = IsDarkTheme ? AppTheme.Dark : AppTheme.Light;
        var coverSize = (CoverSize)CoverSizeIndex;

        _settings.UpdateAppearanceSettings(theme, coverSize);

        _settings.UpdateLibrarySettings(
            (LibrarySortOrder)SortOrderIndex);

        _settings.UpdateGeneralSettings(
            (AppLanguage)LanguageIndex,
            _progressSaveIntervals[ProgressSaveIntervalIndex]);

        _theme.ApplyTheme(theme);
        _theme.ApplyCoverSize(coverSize);

        await _updateSettings.ExecuteAsync(_settings);
    }
}