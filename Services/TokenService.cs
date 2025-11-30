using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MarketingAPI.Models;
using Microsoft.IdentityModel.Tokens;

namespace MarketingAPI.Services;

/// <summary>
/// Service for generating JWT tokens.
/// </summary>
/// <param name="configuration">The application configuration.</param>
public class TokenService(IConfiguration configuration) : ITokenService
{
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>The generated JWT token string.</returns>
    public string GenerateToken(User user)
    {
        IConfigurationSection jwtSettings = _configuration.GetSection("JwtSettings");
        string secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

        string? issuer = jwtSettings["Issuer"];
        string? audience = jwtSettings["Audience"];
        int expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        Claim[] claims =
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var token = new JwtSecurityToken
        (
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}