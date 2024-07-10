using LibGit2Sharp;
using ProjectExplorer.Models;
using System.Collections;
using System.IO;

namespace ProjectExplorer.Business;

/// <summary>
/// Provides functions for the interaction with the git data
/// </summary>
internal static class GitHelper
{
    /// <summary>
    /// Loads the project information of the desired projects
    /// </summary>
    public static void LoadRepoInformation()
    {
        foreach (var entry in SettingsManager.Settings.Projects)
        {
            entry.GitDirectory = DetermineGitFolder(entry.Path);

            if (string.IsNullOrEmpty(entry.GitDirectory))
                continue; // Nothing was found

            LoadGitInformation(entry);
        }
    }

    /// <summary>
    /// Determines the path of the GIT folder (<c>.git</c>)
    /// </summary>
    /// <param name="path">The path of the "root" folder</param>
    /// <returns>The path of the GIT folder (if any exists)</returns>
    private static string DetermineGitFolder(string path)
    {
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return string.Empty;

        var fileInfo = new FileInfo(path);

        // Get the directory
        var dir = fileInfo.Directory;
        return dir == null
            ? string.Empty // can't find anything
            : DetermineFolder(dir); // Check if there is any directory with the name ".git"

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
    /// Loads the GIT information
    /// </summary>
    /// <param name="project">The desired project</param>
    private static void LoadGitInformation(ProjectEntry project)
    {
        using var repo = new Repository(project.GitDirectory);

        project.FriendlyName = repo.Head.FriendlyName;
        project.LastCheck = DateTime.Now;

        var status = repo.RetrieveStatus(new StatusOptions { IncludeIgnored = false });
        var diffFiles = GetDiffFiles(status);
        project.DiffFiles = diffFiles;

        // Get the last commit
        var lastCommit = repo.Commits.FirstOrDefault(); // Get the first entry (the entries are ordered descending)
        project.LastCommit = lastCommit != null ? $"{lastCommit.Author.Name} - {lastCommit.Author.When:yyyy-MM-dd HH:mm:ss}" : "undefined";
    }

    /// <summary>
    /// Gets the diff files
    /// </summary>
    /// <param name="status">The status</param>
    /// <returns>The list with the diff files</returns>
    private static List<DiffFileEntry> GetDiffFiles(IEnumerable? status)
    {
        if (status == null)
            return [];

        var result = new List<DiffFileEntry>();

        var properties = typeof(RepositoryStatus).GetProperties();

        foreach (var property in properties)
        {
            if (property.PropertyType != typeof(IEnumerable<StatusEntry>))
                continue;

            var value = property.GetValue(status);
            if (value is not IEnumerable<StatusEntry> entries)
                continue;

            result.AddRange(entries.Select(s => new DiffFileEntry(s.FilePath, s.State)));
        }

        return result;
    }
}