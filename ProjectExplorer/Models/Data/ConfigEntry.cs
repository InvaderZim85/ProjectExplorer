using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectExplorer.Models.Data;

/// <summary>
/// Represents a configuration entry
/// </summary>
[Table("Configuration")]
internal sealed class ConfigEntry
{
    /// <summary>
    /// Gets or sets the internal id
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the key of the entry
    /// </summary>
    [Required]
    public int Key { get; set; }

    /// <summary>
    /// Gets or sets the value
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Value { get; set; } = string.Empty;
}