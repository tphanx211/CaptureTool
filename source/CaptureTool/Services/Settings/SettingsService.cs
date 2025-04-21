using System.IO;
using System.Text.Json;

namespace CaptureTool.Services.Settings;

public class SettingsService : ISettingsService
{
    private static readonly string DirectoryPath = @"C:\ProgramData\CaptureTool";
    private static readonly string FilePath = Path.Combine(DirectoryPath, "appsettings.json");

    private AppSettings? _settings;
    public AppSettings Settings => _settings ??= Load();

    private AppSettings Load()
    {
        if (!Directory.Exists(DirectoryPath))
            Directory.CreateDirectory(DirectoryPath);

        if (!File.Exists(FilePath))
        {
            var defaultSettings = new AppSettings();
            File.WriteAllText(FilePath, JsonSerializer.Serialize(defaultSettings, new JsonSerializerOptions { WriteIndented = true }));
            return defaultSettings;
        }

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }

    public void Save()
    {
        if (_settings is not null)
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}