using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Domain.Entities;

/// <summary>
/// Configurações globais do aplicativo, persistidas como uma única linha (singleton).
/// </summary>
/// <remarks>
/// Como há apenas um conjunto de preferências por instalação, a entidade usa um
/// <see cref="Id"/> fixo (<see cref="SingletonId"/>). O <see cref="CreateDefault"/> define
/// os valores usados na primeira execução, antes de qualquer ajuste pelo usuário.
/// </remarks>
public class AppSettings
{
    public static readonly Guid SingletonId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public Guid Id { get; private set; }

    public double DefaultZoom { get; private set; }

    public ReaderFitMode FitMode { get; private set; }

    public AppTheme Theme { get; private set; }

    public CoverSize CoverSize { get; private set; }

    public LibrarySortOrder SortOrder { get; private set; }

    public AppLanguage Language { get; private set; }

    public int ProgressSaveIntervalSeconds { get; private set; }

    // Construtor parameterless usado apenas pelo EF Core.
    private AppSettings()
    {
    }

    public AppSettings(
        double defaultZoom,
        ReaderFitMode fitMode,
        AppTheme theme,
        CoverSize coverSize,
        LibrarySortOrder sortOrder,
        AppLanguage language,
        int progressSaveIntervalSeconds)
    {
        Id = SingletonId;
        DefaultZoom = defaultZoom;
        FitMode = fitMode;
        Theme = theme;
        CoverSize = coverSize;
        SortOrder = sortOrder;
        Language = language;
        ProgressSaveIntervalSeconds = progressSaveIntervalSeconds;
    }

    public static AppSettings CreateDefault() =>
        new(
            1.0,
            ReaderFitMode.Page,
            AppTheme.Dark,
            CoverSize.Medium,
            LibrarySortOrder.Title,
            AppLanguage.Portuguese,
            0);

    public void UpdateReaderSettings(double defaultZoom, ReaderFitMode fitMode)
    {
        DefaultZoom = defaultZoom;
        FitMode = fitMode;
    }

    public void UpdateAppearanceSettings(AppTheme theme, CoverSize coverSize)
    {
        Theme = theme;
        CoverSize = coverSize;
    }

    public void UpdateLibrarySettings(LibrarySortOrder sortOrder)
    {
        SortOrder = sortOrder;
    }

    public void UpdateGeneralSettings(AppLanguage language, int progressSaveIntervalSeconds)
    {
        Language = language;
        ProgressSaveIntervalSeconds = progressSaveIntervalSeconds;
    }
}