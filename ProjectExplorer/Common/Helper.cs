using System.Diagnostics;
using System.IO;
using ProjectExplorer.Models.Data;
using Serilog;

namespace ProjectExplorer.Common;

/// <summary>
/// Provides several helper functions
/// </summary>
internal static class Helper
{
    /// <summary>
    /// Opens the desired file in the desired program
    /// </summary>
    /// <param name="program">The path of the program</param>
    /// <param name="path">The path of the file / folder</param>
    public static void OpenIn(string program, string path)
    {
        if (!File.Exists(program) || (!File.Exists(path) && !Directory.Exists(path)))
            return;

        var startInfo = new ProcessStartInfo(program)
        {
            Arguments = path
        };

        Process.Start(startInfo);
    }

    /// <summary>
    /// Cleans the bin / obj folder
    /// </summary>
    /// <param name="project">The project</param>
    /// <returns><see langword="true"/> when the folder were cleaned, otherwise <see langword="false"/></returns>
    public static bool Clean(ProjectEntry project)
    {
        // Step 1: Determine the path of the folder
        project.BinFolder = DetermineFolder(project, "bin");
        project.ObjFolder = DetermineFolder(project, "obj");

        if (!Clean(project.BinFolder))
            return false;

        if (!Clean(project.ObjFolder))
            return false;

        return true;
    }

    /// <summary>
    /// Cleans the desired directory
    /// </summary>
    /// <param name="path">The path of the directory</param>
    /// <returns><see langword="true"/> when the folder were cleaned, otherwise <see langword="false"/></returns>
    private static bool Clean(string path)
    {
        if (!Directory.Exists(path))
            return true;

        try
        {
            Directory.Delete(path, true);

            return true;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "An error has occurred while cleaning the folder '{path}'", path);
            return false;
        }
    }

    /// <summary>
    /// Tries to determine the <c>.git</c> folder of the specified solution file
    /// </summary>
    /// <param name="project">The project file</param>
    /// <param name="folderName">The name of the folder which should be loaded</param>
    /// <returns>The path of the </returns>
    public static string DetermineFolder(ProjectEntry project, string folderName)
    {
        if (string.IsNullOrEmpty(project.Path) || !File.Exists(project.Path))
            return string.Empty;

        var fileInfo = new FileInfo(project.Path);

        // Get the directory
        var dir = fileInfo.Directory;
        return dir == null 
            ? string.Empty 
            : SearchFolder(dir);
        
        string SearchFolder(DirectoryInfo rootDir)
        {
            var subDirs = rootDir.GetDirectories();
            foreach (var subDir in subDirs)
            {
                if (subDir.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase))
                    return subDir.FullName;

                var result = SearchFolder(subDir);
                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// Opens the desired path in the windows explorer
    /// </summary>
    /// <param name="path">The path of the file</param>
    public static void OpenInExplorer(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
            return;

        var arguments = $"/n, /e, \"{path}\"";
        Process.Start("explorer.exe", arguments);
    }
}