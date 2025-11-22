namespace MarketingAPI.Models.DTOs;

public class LoginResponseDto
{
    /// <summary>
    /// The returned token to use with authenticated requests.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The username of the logged in user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The email of the logged in user.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}