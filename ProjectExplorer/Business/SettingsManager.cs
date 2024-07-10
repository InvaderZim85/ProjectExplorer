using System.IO;
using Newtonsoft.Json;
using ProjectExplorer.Models;

namespace ProjectExplorer.Business;

/// <summary>
/// Provides the functions for the interaction with the data
/// </summary>
internal static class SettingsManager
{
    /// <summary>
    /// Contains the path of the settings file
    /// </summary>
    private static readonly string SettingsFile = Path.Combine(AppContext.BaseDirectory, "Settings.json");

    /// <summary>
    /// Backing field for <see cref="Settings"/>
    /// </summary>
    private static Settings? _settings;

    /// <summary>
    /// Gets the settings
    /// </summary>
    public static Settings Settings
    {
        get
        {
            _settings ??= LoadSettings();
            return _settings;
        }
    }

    /// <summary>
    /// Loads the settings and stores them into <see cref="Settings"/>
    /// </summary>
    private static Settings LoadSettings()
    {
        if (!File.Exists(SettingsFile))
            return new Settings();

        var content = File.ReadAllText(SettingsFile);

        return JsonConvert.DeserializeObject<Settings>(content) ?? new Settings();
    }

    /// <summary>
    /// Saves the current settings
    /// </summary>
    /// <returns>The awaitable task</returns>
    public static void SaveSettings()
    {
        Settings.LastSaveDateTime = DateTime.Now;

        var content = JsonConvert.SerializeObject(Settings, Formatting.Indented);

        File.WriteAllText(SettingsFile, content);
    }
}