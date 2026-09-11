using System.Resources;

namespace MinhaEstante.Presentation.Resources;

public static class Strings
{
    private static readonly ResourceManager Manager =
        new("MinhaEstante.Presentation.Resources.Strings", typeof(Strings).Assembly);

    public static string LibraryTitle => Get("LibraryTitle");
    public static string ImportBook => Get("ImportBook");
    public static string EmptyLibrary => Get("EmptyLibrary");
    public static string Open => Get("Open");
    public static string Edit => Get("Edit");
    public static string Remove => Get("Remove");
    public static string ReaderBack => Get("ReaderBack");
    public static string ReaderPrevious => Get("ReaderPrevious");
    public static string ReaderNext => Get("ReaderNext");
    public static string SectionLibrary => Get("SectionLibrary");
    public static string SectionSettings => Get("SectionSettings");
    public static string SectionAbout => Get("SectionAbout");

    public static string Cancel => Get("Cancel");
    public static string Save => Get("Save");
    public static string Import => Get("Import");
    public static string Title => Get("Title");
    public static string Author => Get("Author");
    public static string Genre => Get("Genre");

    public static string ReaderSection => Get("ReaderSection");
    public static string ReaderSectionSubtitle => Get("ReaderSectionSubtitle");
    public static string DefaultZoom => Get("DefaultZoom");
    public static string FitMode => Get("FitMode");
    public static string AppearanceSection => Get("AppearanceSection");
    public static string AppearanceSectionSubtitle => Get("AppearanceSectionSubtitle");
    public static string Theme => Get("Theme");
    public static string ThemeDark => Get("ThemeDark");
    public static string ThemeLight => Get("ThemeLight");
    public static string CoverSize => Get("CoverSize");
    public static string LibrarySortSubtitle => Get("LibrarySortSubtitle");
    public static string SortOrder => Get("SortOrder");
    public static string StorageSection => Get("StorageSection");
    public static string StorageSectionSubtitle => Get("StorageSectionSubtitle");
    public static string LibraryFolder => Get("LibraryFolder");
    public static string BrowseFolder => Get("BrowseFolder");
    public static string ChooseLibraryFolder => Get("ChooseLibraryFolder");
    public static string MoveDataHint => Get("MoveDataHint");
    public static string Cache => Get("Cache");
    public static string ClearPageCache => Get("ClearPageCache");
    public static string GeneralSection => Get("GeneralSection");
    public static string GeneralSectionSubtitle => Get("GeneralSectionSubtitle");
    public static string Language => Get("Language");
    public static string SaveProgress => Get("SaveProgress");

    public static string FitToPage => Get("FitToPage");
    public static string FitToWidth => Get("FitToWidth");
    public static string FullHeight => Get("FullHeight");
    public static string CoverSmall => Get("CoverSmall");
    public static string CoverMedium => Get("CoverMedium");
    public static string CoverLarge => Get("CoverLarge");
    public static string SortByTitle => Get("SortByTitle");
    public static string SortByAuthor => Get("SortByAuthor");
    public static string SortByDateAdded => Get("SortByDateAdded");
    public static string SortByLastAccessed => Get("SortByLastAccessed");
    public static string EveryChange => Get("EveryChange");
    public static string Every5Seconds => Get("Every5Seconds");
    public static string Every10Seconds => Get("Every10Seconds");
    public static string Every30Seconds => Get("Every30Seconds");

    public static string LanguageAppliedOnRestart => Get("LanguageAppliedOnRestart");
    public static string DataMovedRestart => Get("DataMovedRestart");
    public static string ErrorMoving => Get("ErrorMoving");
    public static string CacheCleared => Get("CacheCleared");

    public static string SelectPdfOrCbz => Get("SelectPdfOrCbz");
    public static string Importing => Get("Importing");
    public static string ImportedSuccessfully => Get("ImportedSuccessfully");
    public static string ImportError => Get("ImportError");
    public static string ImportSubtitle => Get("ImportSubtitle");
    public static string SelectFile => Get("SelectFile");
    public static string NoFileSelected => Get("NoFileSelected");
    public static string SelectBook => Get("SelectBook");
    public static string DigitalBooks => Get("DigitalBooks");

    public static string EditInfo => Get("EditInfo");
    public static string AboutDescription => Get("AboutDescription");
    public static string Version => Get("Version");

    public static string RemoveBookTitle => Get("RemoveBookTitle");
    public static string RemoveBookQuestion => Get("RemoveBookQuestion");
    public static string DeleteFileAlso => Get("DeleteFileAlso");

    private static string Get(string key) => Manager.GetString(key) ?? $"#{key}";
}