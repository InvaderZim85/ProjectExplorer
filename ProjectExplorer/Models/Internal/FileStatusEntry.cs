using System.IO;
using LibGit2Sharp;

namespace ProjectExplorer.Models.Internal;

/// <summary>
/// Provides the information about the "diff" file
/// </summary>
/// <param name="path">The path of the file</param>
/// <param name="status">The file status</param>
public sealed class FileStatusEntry(string path, FileStatus status)
{
    /// <summary>
    /// Gets the name of the file
    /// </summary>
    public string Name { get; } = Path.GetFileName(path);

    /// <summary>
    /// Gets the path of the file
    /// </summary>
    public string FilePath { get; } = path;

    /// <summary>
    /// Gets the type of the file
    /// </summary>
    public string Status => status switch
    {
        FileStatus.Nonexistent => "Non existent",
        FileStatus.Unaltered => "Unmodified",
        FileStatus.NewInIndex => "Added",
        FileStatus.ModifiedInIndex or FileStatus.ModifiedInWorkdir => "Modified",
        FileStatus.DeletedFromIndex or FileStatus.DeletedFromWorkdir => "Deleted",
        FileStatus.RenamedInIndex or FileStatus.RenamedInWorkdir => "Renamed",
        FileStatus.TypeChangeInIndex or FileStatus.TypeChangeInWorkdir => "Type changed",
        FileStatus.NewInWorkdir => "New file (not yet added)",
        FileStatus.Unreadable => "Unreadable",
        FileStatus.Ignored => "Ignored",
        FileStatus.Conflicted => "Conflict",
        _ => "undefined"
    };
}