using Newtonsoft.Json.Linq;
using ProjectExplorer.Common.Enums;
using ProjectExplorer.Data;
using ProjectExplorer.Models.Data;

namespace ProjectExplorer.Business;

/// <summary>
/// Provides the functions for the interaction with the settings.
/// </summary>
internal static class ConfigManager
{
    /// <summary>
    /// Loads the desired settings value
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="key">The key</param>
    /// <param name="defaultValue">The default value</param>
    /// <returns>The settings value</returns>
    public static T LoadValue<T>(ConfigKey key, T defaultValue)
    {
        using var context = CreateContext();

        var entry = context.Configuration.FirstOrDefault(f => f.Key == (int)key);
        if (entry == null)
            return defaultValue;

        try
        {
            return (T)Convert.ChangeType(entry.Value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// Saves a configuration value
    /// </summary>
    /// <param name="key">The key of the value</param>
    /// <param name="value">The value which should be saved</param>
    /// <returns>The awaitable task</returns>
    public static async Task SaveValueAsync(ConfigKey key, object value)
    {
        await using var context = CreateContext();

        var entry = context.Configuration.FirstOrDefault(f => f.Key == (int)key);
        if (entry == null)
        {
            await context.Configuration.AddAsync(new ConfigEntry
            {
                Key = (int)key,
                Value = value.ToString() ?? string.Empty
            });
        }
        else
        {
            entry.Value = value.ToString() ?? string.Empty;
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Saves multiple configuration values
    /// </summary>
    /// <param name="values">The list with the values which should be saved</param>
    /// <returns>The awaitable task</returns>
    public static async Task SaveValuesAsync(SortedList<ConfigKey, object> values)
    {
        await using var context = CreateContext();

        foreach (var entry in values)
        {
            var dbEntry = context.Configuration.FirstOrDefault(f => f.Key == (int)entry.Key);
            if (dbEntry == null)
            {
                await context.Configuration.AddAsync(new ConfigEntry
                {
                    Key = (int)entry.Key,
                    Value = entry.Value.ToString() ?? string.Empty
                });
            }
            else
            {
                dbEntry.Value = entry.Value.ToString() ?? string.Empty;
            }
        }

        await context.SaveChangesAsync();
        
    }

    /// <summary>
    /// Creates a new instance of the <see cref="AppDbContext"/>.
    /// </summary>
    /// <returns>The instance of the context</returns>
    private static AppDbContext CreateContext()
    {
        return new AppDbContext(true);
    }
}