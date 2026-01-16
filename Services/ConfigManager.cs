using System.Text.Json;
using MinimalPomodoro.Models;

namespace MinimalPomodoro.Services;

public static class ConfigManager
{
    private static readonly string FolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "MinimalPomodoro"
    );
    private static readonly string FilePath = Path.Combine(FolderPath, "config.json");

    public static AppConfig Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
            }
        }
        catch (Exception)
        {
            // Silently fail and return default
        }
        return new AppConfig();
    }

    public static void Save(AppConfig config)
    {
        try
        {
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
        catch (Exception)
        {
            // Handle error or log
        }
    }

    public static void Reset()
    {
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }

    public static string GetConfigPath() => FolderPath;
}
