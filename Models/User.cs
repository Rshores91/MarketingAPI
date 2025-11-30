using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketingAPI.Models;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The username of the user.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The email address of the user.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The hashed password of the user.
    /// </summary>
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the user was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The date and time when the user last logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Indicates whether the user account is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}