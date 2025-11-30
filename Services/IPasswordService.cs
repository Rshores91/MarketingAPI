using MarketingAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace MarketingAPI.Services;

/// <summary>
/// Interface for password hashing and verification services.
/// </summary>
public interface IPasswordService
{
    /// <summary>
    /// Hashes a password for a given user.
    /// </summary>
    /// <param name="user">The user object.</param>
    /// <param name="password">The plain text password.</param>
    /// <returns>The hashed password.</returns>
    public string HashPassword(User user, string password);

    /// <summary>
    /// Verifies a password against a stored hash.
    /// </summary>
    /// <param name="user">The user object.</param>
    /// <param name="password">The plain text password.</param>
    /// <param name="passwordHash">The stored password hash.</param>
    /// <returns>The result of the password verification.</returns>
    public PasswordVerificationResult VerifyPassword(User user, string password, string passwordHash);
}