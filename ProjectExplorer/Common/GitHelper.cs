using LibGit2Sharp;
using ProjectExplorer.Business;
using ProjectExplorer.Common.Enums;
using ProjectExplorer.Models.Data;
using ProjectExplorer.Models.Internal;
using Serilog;
using System.Collections;
using System.IO;

namespace ProjectExplorer.Common;

/// <summary>
/// Provides several functions for the interaction with the git data
/// </summary>
internal static class GitHelper
{
    /// <summary>
    /// Loads the branch information of the specified project
    /// </summary>
    /// <param name="project">The project</param>
    public static void LoadBranchInformation(ProjectEntry project)
    {
        // Try to determine the .git folder
        DetermineGitFolder(project);

        if (!Directory.Exists(project.GitFolder))
            return; // Nothing to load here...

        using var repo = new Repository(project.GitFolder);

        project.Branch = repo.Head.FriendlyName;
        project.Branches = repo.Branches.Select(s => new BranchEntry(s)).ToList();
        project.LastCheck = DateTime.Now;

        var status = repo.RetrieveStatus(new StatusOptions { IncludeIgnored = false });
        var diffFiles = GetDiffFiles(status);
        project.Files = diffFiles;
        project.StatusInfo =
            $"+{status.Added.Count()} ~{status.Staged.Count()} -{status.Removed.Count()} | " +
            $"+{status.Untracked.Count()} ~{status.Modified.Count()} -{status.Missing.Count()} | " +
            $"i{status.Ignored.Count()}";

        // Get the last commit
        var lastCommit = repo.Commits.FirstOrDefault(); // Get the first entry (the entries are ordered descending)
        project.LastCommit = lastCommit != null ? $"{lastCommit.Author.Name} - {lastCommit.Author.When:yyyy-MM-dd HH:mm:ss}" : "undefined";
    }

    /// <summary>
    /// Gets the diff files
    /// </summary>
    /// <param name="status">The status</param>
    /// <returns>The list with the diff files</returns>
    private static List<FileStatusEntry> GetDiffFiles(IEnumerable? status)
    {
        if (status == null)
            return [];

        var result = new List<FileStatusEntry>();

        var properties = typeof(RepositoryStatus).GetProperties();

        foreach (var property in properties)
        {
            if (property.PropertyType != typeof(IEnumerable<StatusEntry>))
                continue;

            var value = property.GetValue(status);
            if (value is not IEnumerable<StatusEntry> entries)
                continue;

            result.AddRange(entries.Select(s => new FileStatusEntry(s.FilePath, s.State)));
        }

        return result;
    }

    /// <summary>
    /// Tries to determine the <c>.git</c> folder of the specified solution file
    /// </summary>
    /// <param name="project">The project file</param>
    /// <returns>The path of the </returns>
    private static void DetermineGitFolder(ProjectEntry project)
    {
        if (string.IsNullOrEmpty(project.Path) || !File.Exists(project.Path))
            return;

        var fileInfo = new FileInfo(project.Path);

        // Get the directory
        var dir = fileInfo.Directory;
        if (dir == null)
            return;

        // Set the "root" folder
        project.Folder = dir.FullName;

        // Set the git folder
        project.GitFolder = DetermineFolder(dir);

        return;

        static string DetermineFolder(DirectoryInfo rootDir)
        {
            while (true)
            {
                var subDirs = rootDir.GetDirectories(".git");
                if (subDirs.Length != 0) return subDirs[0].FullName;

                var parent = rootDir.Parent;

                if (parent == null)
                    return string.Empty;

                // Set the parent as the new root dir
                rootDir = parent;
            }
        }
    }

    /// <summary>
    /// Performs a <c>git-clean</c> and removes all untracked files
    /// </summary>
    /// <param name="project">The project which should be cleaned</param>
    /// <returns><see langword="true"/> when everything was successful, otherwise <see langword="false"/></returns>
    public static bool Clean(ProjectEntry project)
    {
        if (!Directory.Exists(project.GitFolder))
            return true; // Nothing to load...

        using var repo = new Repository(project.GitFolder);

        var clearUntracked = ConfigManager.LoadValue(ConfigKey.GitCleanUntracked, true);
        var clearIgnored = ConfigManager.LoadValue(ConfigKey.GitCleanIgnored, false);

        // Get all "untracked" files
        var status = repo.RetrieveStatus(new StatusOptions
        {
            IncludeUntracked = clearUntracked,
            IncludeIgnored = clearIgnored
        });

        if (clearUntracked)
        {
            foreach (var entry in status.Untracked)
            {
                DeleteFile(entry.FilePath);
            }
        }

        if (clearIgnored)
        {
            foreach (var entry in status.Ignored)
            {
                DeleteFile(entry.FilePath);
            }
        }

        return true;

        void DeleteFile(string path)
        {
            var tmpPath = Path.Combine(repo.Info.WorkingDirectory, path.Replace("/", "\\"));
            if (!File.Exists(tmpPath))
                return;

            try
            {
                File.Delete(tmpPath);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Git-Clean: Can't delete file '{path}'.", tmpPath);
            }
        }
    }
}