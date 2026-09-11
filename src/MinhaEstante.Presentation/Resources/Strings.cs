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

    private static string Get(string key) => Manager.GetString(key) ?? $"#{key}";
}