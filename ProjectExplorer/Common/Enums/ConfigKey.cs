namespace ProjectExplorer.Common.Enums;

/// <summary>
/// Provides the different configuration keys
/// </summary>
internal enum ConfigKey
{
    /// <summary>
    /// The path of visual studio
    /// </summary>
    VisualStudioPath = 1,

    /// <summary>
    /// The desired color theme
    /// </summary>
    ColorTheme = 2,

    /// <summary>
    /// The path of visual studio code.
    /// </summary>
    VisualStudioCodePath = 3,

    /// <summary>
    /// The option to delete also <i>untracked</i> files
    /// </summary>
    GitCleanUntracked = 4,

    /// <summary>
    /// The option to delete also <i>ignored</i> files
    /// </summary>
    GitCleanIgnored = 5
}