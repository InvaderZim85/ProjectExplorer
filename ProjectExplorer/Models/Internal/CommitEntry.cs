using LibGit2Sharp;

namespace ProjectExplorer.Models.Internal;

/// <summary>
/// Represents a commit
/// </summary>
/// <param name="commit">The commit</param>
public sealed class CommitEntry(Commit commit)
{
    /// <summary>
    /// Gets the commit message
    /// </summary>
    public string Message { get; } = commit.Message;

    /// <summary>
    /// Gets the name of the author
    /// </summary>
    public string Author { get; } = commit.Author.Name;

    /// <summary>
    /// Gets the commit date
    /// </summary>
    public DateTime CommitDate { get; } = commit.Author.When.DateTime;
}