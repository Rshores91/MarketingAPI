using MarketingAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace MarketingAPI.Services;

/// <summary>
/// Service for handling password hashing and verification.
/// </summary>
/// <param name="passwordHasher">The password hasher implementation.</param>
public class PasswordService(IPasswordHasher<User> passwordHasher) : IPasswordService
{
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    /// <summary>
    /// Hashes a password for a given user.
    /// </summary>
    /// <param name="user">The user object.</param>
    /// <param name="password">The plain text password.</param>
    /// <returns>The hashed password.</returns>
    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    /// <summary>
    /// Verifies a password against a stored hash.
    /// </summary>
    /// <param name="user">The user object.</param>
    /// <param name="password">The plain text password.</param>
    /// <param name="passwordHash">The stored password hash.</param>
    /// <returns>The result of the password verification.</returns>
    public PasswordVerificationResult VerifyPassword(User user, string password, string passwordHash)
    {
        return _passwordHasher.VerifyHashedPassword(user, passwordHash, password);
    }
}