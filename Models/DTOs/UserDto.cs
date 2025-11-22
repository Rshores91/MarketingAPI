namespace MarketingAPI.Models.DTOs;

public class UserDto
{
    /// <summary>
    /// The users id.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The username of the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The email of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The date and time that the user account was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The last time that the user was logged in.
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Whether or not the user is currently active.
    /// </summary>
    public bool IsActive { get; set; }
}