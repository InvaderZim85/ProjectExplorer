using LibGit2Sharp;
using ZimLabs.TableCreator;

namespace ProjectExplorer.Models.Internal;

/// <summary>
/// Represents a branch entry
/// </summary>
/// <param name="branch">The original branch</param>
public sealed class BranchEntry(Branch branch)
{
    /// <summary>
    /// Gets the name of the branch
    /// </summary>
    [Appearance(EncapsulateContent = true)]
    public string Name { get; } = branch.FriendlyName;

    /// <inheritdoc cref="Branch.IsCurrentRepositoryHead"/>
    public bool IsCurrentBranch { get; } = branch.IsCurrentRepositoryHead;

    /// <inheritdoc cref="Branch.IsRemote"/>
    public bool IsRemote { get; } = branch.IsRemote;

    /// <inheritdoc cref="Branch.IsTracking"/>
    public bool IsTracking { get; } = branch.IsTracking;

    /// <summary>
    /// Gets the list with the commits
    /// </summary>
    [Appearance(true)]
    public List<CommitEntry> Commits { get; } = branch.Commits.Select(s => new CommitEntry(s)).ToList();

    /// <summary>
    /// Gets the last commit date
    /// </summary>
    [Appearance(Format = "yyyy-MM-dd HH:mm:ss")]
    public DateTime LastCommit => Commits.Max(m => m.CommitDate);
}