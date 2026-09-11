using System.Globalization;
using MinhaEstante.Domain.Enums;

namespace MinhaEstante.Presentation.Services;

public static class LocalizationService
{
    public static void Apply(AppLanguage language)
    {
        var culture = language == AppLanguage.English
            ? new CultureInfo("en")
            : new CultureInfo("pt-BR");

        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
    }
}