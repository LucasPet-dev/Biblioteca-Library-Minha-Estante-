using System.Text.Json;

namespace MinhaEstante.Infrastructure.Storage;

public static class AppConfig
{
    private static readonly string AppFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MinhaEstante");

    private static readonly string ConfigFilePath = Path.Combine(AppFolder, "app.config.json");

    public static string ReadDataDirectory()
    {
        try
        {
            if (File.Exists(ConfigFilePath))
            {
                var dto = JsonSerializer.Deserialize<AppConfigDto>(File.ReadAllText(ConfigFilePath));

                if (!string.IsNullOrWhiteSpace(dto?.DataDirectory))
                {
                    return dto.DataDirectory;
                }
            }
        }
        catch
        {
            // Ponteiro corrompido/ilegível: cai para o diretório padrão.
        }

        return FileStorage.DefaultDataDirectory;
    }

    public static void WriteDataDirectory(string dataDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);

        Directory.CreateDirectory(AppFolder);

        var dto = new AppConfigDto { DataDirectory = dataDirectory };
        File.WriteAllText(ConfigFilePath, JsonSerializer.Serialize(dto));
    }

    private sealed class AppConfigDto
    {
        public string? DataDirectory { get; set; }
    }
}