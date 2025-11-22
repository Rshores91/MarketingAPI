using System.ComponentModel.DataAnnotations;

namespace MarketingAPI.Models.DTOs;

public class RegisterDto
{
    /// <summary>
    /// The username to attempt registration with.
    /// </summary>
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The email to attempt registration with.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The password to attempt registration with.
    /// </summary>
    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}