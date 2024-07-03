using System;
using System.IO;
using Newtonsoft.Json;

namespace GenerateCertificateUI;

public static class SettingsService
{
    private static readonly string SettingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GenerateCertificateUI",
        "settings.json");

    static SettingsService()
    {
        EnsureSettingsDirectoryExists();
    }

    private static void EnsureSettingsDirectoryExists()
    {
        string directory = Path.GetDirectoryName(SettingsFilePath)!;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static Settings LoadSettings()
    {
        if (!File.Exists(SettingsFilePath))
        {
            return new Settings("", "");
        }

        var json = File.ReadAllText(SettingsFilePath);
        return JsonConvert.DeserializeObject<Settings>(json);
    }

    public static void SaveSettings(Settings settings)
    {
        var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(SettingsFilePath, json);
    }
}
