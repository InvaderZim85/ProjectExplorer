using System.IO;

namespace ProjectExplorer.Models;

/// <summary>
/// Represents a git repository
/// </summary>
public sealed class ProjectEntry
{
    /// <summary>
    /// Gets or sets the name of the repository
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path of the project
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path of the git directory
    /// </summary>
    public string GitDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets the path of the repository
    /// </summary>
    public string RepoDirectory => string.IsNullOrEmpty(GitDirectory) || !Directory.Exists(GitDirectory)
        ? "/"
        : new DirectoryInfo(GitDirectory).Parent?.FullName ?? "";

    /// <summary>
    /// Gets or sets the friendly name of the branch
    /// </summary>
    public string FriendlyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the repository
    /// </summary>
    public string Status => DiffFiles.Count > 0 ? "Has changes" : "No changes since last commit";

    /// <summary>
    /// Gets or sets the last commit information
    /// </summary>
    public string LastCommit { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list with the diff files
    /// </summary>
    public List<DiffFileEntry> DiffFiles { get; set; } = [];

    /// <summary>
    /// Gets or sets the date / time of the last check
    /// </summary>
    public DateTime LastCheck { get; set; }
}