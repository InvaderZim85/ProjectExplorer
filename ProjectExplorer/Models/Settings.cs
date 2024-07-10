namespace ProjectExplorer.Models;

/// <summary>
/// Provides the different settings
/// </summary>
internal sealed class Settings
{
    /// <summary>
    /// Contains the list with the project directories
    /// </summary>
    public List<ProjectEntry> Projects { get; set; } = [];

    /// <summary>
    /// Gets or sets the path of visual studio
    /// </summary>
    public string VisualStudioPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date / time of the last save
    /// </summary>
    public DateTime LastSaveDateTime { get; set; }
}