using System.IO;
using Microsoft.EntityFrameworkCore;
using ProjectExplorer.Models.Data;

namespace ProjectExplorer.Data;

/// <summary>
/// Provides the functions for the interaction with the database.
/// </summary>
/// <param name="disableTracking"><see langword="true"/> to disable the tracking, otherwise <see langword="false"/>.</param>
internal sealed class AppDbContext(bool disableTracking = false) : DbContext
{
    /// <summary>
    /// Gets or sets the list with the different projects.
    /// </summary>
    public DbSet<ProjectEntry> Projects => Set<ProjectEntry>();

    /// <summary>
    /// Gets or sets the configurations.
    /// </summary>
    public DbSet<ConfigEntry> Configuration => Set<ConfigEntry>();

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "ProjectExplorer.db");

        optionsBuilder.UseSqlite($"Data Source = {path}");

        if (disableTracking)
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectEntry>().HasIndex(c => c.Path).IsUnique();
        modelBuilder.Entity<ConfigEntry>().HasIndex(c => c.Key).IsUnique();
    }
}