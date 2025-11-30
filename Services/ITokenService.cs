using MarketingAPI.Models;

namespace MarketingAPI.Services;

/// <summary>
/// Interface for token generation services.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>The generated JWT token string.</returns>
    string GenerateToken(User user);
}