using CommunityToolkit.Mvvm.ComponentModel;
using ProjectExplorer.Models.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ZimLabs.TableCreator;

namespace ProjectExplorer.Models.Data;

/// <summary>
/// Represents a project entry
/// </summary>
[Table("Project")]
public sealed class ProjectEntry : ObservableObject
{
    /// <summary>
    /// Gets or sets the internal id of the project entry
    /// </summary>
    [Key]
    [Appearance(true)]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the project
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Appearance(EncapsulateContent = true)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Backing field for <see cref="Path"/>
    /// </summary>
    private string _path = string.Empty;

    /// <summary>
    /// Gets or sets the path of the project
    /// </summary>
    [Required]
    [MaxLength(500)]
    [Appearance(EncapsulateContent = true)]
    public string Path
    {
        get => _path;
        set => SetProperty(ref _path, value);
    }

    /// <summary>
    /// Gets or sets the folder which contains the solution file
    /// </summary>
    [NotMapped]
    [Appearance(EncapsulateContent = true)]
    public string Folder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path of the GIT (<c>.git</c>) folder
    /// </summary>
    [NotMapped]
    [Appearance(EncapsulateContent = true)]
    public string GitFolder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path of the <c>bin</c> folder
    /// </summary>
    [NotMapped]
    [Appearance(true)]
    public string BinFolder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path of the <c>obj</c> folder
    /// </summary>
    [NotMapped]
    [Appearance(true)]
    public string ObjFolder { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the "friendly" name of the branch
    /// </summary>
    [NotMapped]
    [Appearance(EncapsulateContent = true)]
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status of the repository
    /// </summary>
    [NotMapped]
    [Appearance(EncapsulateContent = true)]
    public string Status => Files.Count > 0 ? "Has changes" : "No changes since last commit";

    /// <summary>
    /// Gets or sets the status info
    /// </summary>
    [NotMapped]
    [Appearance(EncapsulateContent = true)]
    public string StatusInfo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last commit information
    /// </summary>
    [NotMapped]
    [Appearance(EncapsulateContent = true)]
    public string LastCommit { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list with the changed files
    /// </summary>
    [NotMapped]
    [Appearance(true)]
    public List<FileStatusEntry> Files { get; set; } = [];

    /// <summary>
    /// Gets or sets the list with the branches
    /// </summary>
    [NotMapped]
    [Appearance(true)]
    public List<BranchEntry> Branches { get; set; } = [];

    /// <summary>
    /// Backing field for <see cref="LastCheck"/>
    /// </summary>
    private DateTime _lastCheck;

    /// <summary>
    /// Gets or sets the date / time of the last check
    /// </summary>
    [NotMapped]
    [Appearance(Format = "yyyy-MM-dd HH:mm:ss")]
    public DateTime LastCheck
    {
        get => _lastCheck;
        set => SetProperty(ref _lastCheck, value);
    }
}