using Microsoft.EntityFrameworkCore;
using ProjectExplorer.Common;
using ProjectExplorer.Data;
using ProjectExplorer.Models.Data;

namespace ProjectExplorer.Business;

/// <summary>
/// Provides the functions for the interaction with the data
/// </summary>
internal sealed class DataManager : IDisposable
{
    /// <summary>
    /// Contains the value which indicates if the class was already disposed
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Contains the instance for the interaction with the data
    /// </summary>
    private readonly AppDbContext _context = new();

    /// <summary>
    /// Gets the list with the projects
    /// </summary>
    public List<ProjectEntry> Projects { get; private set; } = [];

    /// <summary>
    /// Loads all available projects and stores them into <see cref="Projects"/>
    /// </summary>
    /// <returns>The awaitable task</returns>
    public async Task LoadProjectsAsync()
    {
        Projects = await _context.Projects.ToListAsync();

        // Load the branch information
        foreach (var project in Projects)
        {
            GitHelper.LoadBranchInformation(project);
        }
    }

    /// <summary>
    /// Adds a new project to the list
    /// </summary>
    /// <param name="project">The project which should be added</param>
    /// <returns>The awaitable task</returns>
    public async Task AddProjectAsync(ProjectEntry project)
    {
        // Determine the git information
        GitHelper.LoadBranchInformation(project);

        // Add the project
        await _context.Projects.AddAsync(project);

        await _context.SaveChangesAsync();

        Projects.Add(project);
    }

    /// <summary>
    /// Deletes the desired project
    /// </summary>
    /// <param name="project">The project which should be deleted</param>
    /// <returns>The awaitable task</returns>
    public async Task DeleteProjectAsync(ProjectEntry project)
    {
        // Remove the project from the database
        _context.Projects.Remove(project);

        // Remove the project from the list
        Projects.Remove(project);

        // Save the changes
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Saves the current changes
    /// </summary>
    /// <returns>The awaitable task</returns>
    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if the desired project already exists in the list
    /// </summary>
    /// <param name="id">The id of the project</param>
    /// <param name="filepath">The path of the file</param>
    /// <returns><see langword="true"/> when the project already exists, otherwise <see langword="false"/></returns>
    public Task<bool> ProjectExistsAsync(int id, string filepath)
    {
        return _context.Projects.AnyAsync(a => (id == 0 || a.Id != id) && a.Path.Equals(filepath));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _context.Dispose();

        _disposed = true;
    }
}