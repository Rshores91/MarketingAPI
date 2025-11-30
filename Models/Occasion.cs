using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingAPI.Models;

/// <summary>
/// Occasion model
/// </summary>
public class Occasion
{
    /// <summary>
    /// The unique identifier for the occasion.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The name of the occasion.
    /// </summary>
    [Required]
    public string OccasionName { get; set; } = string.Empty;

    /// <summary>
    /// The type of the occasion.
    /// </summary>
    [Required]
    public string OccasionType { get; set; } = string.Empty;

    /// <summary>
    /// The list of attendees for the occasion.
    /// </summary>
    public List<string> OccasionAttendees { get; set; } = [];

    /// <summary>
    /// The date and time of the occasion.
    /// </summary>
    [Required]
    public DateTime OccasionDateTime { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// The duration of the occasion.
    /// </summary>
    public string OccasionDuration { get; set; } = string.Empty;
}