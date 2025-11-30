using System.Security.Claims;
using MarketingAPI.Data;
using MarketingAPI.Models;
using MarketingAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketingAPI.Controllers;

/// <summary>
/// Main controller for the API
/// </summary>
/// <param name="context">Database context</param>
/// <param name="logger">Main logger</param>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(ApplicationDbContext context, ILogger<UsersController> logger) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<UsersController> _logger = logger;

    /// <summary>
    /// Gets the current user
    /// </summary>
    /// <returns>The user DTO based on the token given.</returns>
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        Claim? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId))
            return Unauthorized();

        User? user = await _context.Users.FindAsync(userId);
        if (user is null)
            return NotFound();

        UserDto userDto = new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive
        };

        return Ok(userDto);
    }

    /// <summary>
    /// Gets all registered users
    /// </summary>
    /// <returns>A list of registered users</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        List<UserDto> users = await _context.Users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt,
            IsActive = u.IsActive
        }).ToListAsync();

        return Ok(users);
    }

    /// <summary>
    /// Gets a user based on the given Id
    /// </summary>
    /// <param name="id">The id of the user to get</param>
    /// <returns>The user DTO based on the id.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        User? user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound();

        UserDto userDto = new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive
        };

        return Ok(userDto);
    }

    /// <summary>
    /// Deletes a user based on the Id
    /// </summary>
    /// <param name="id">The id of the user to delete</param>
    /// <returns>No Content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        User? user = await _context.Users.FindAsync(id);
        if (user is null)
            return NotFound();
        

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User deleted: {Username}", user.Username);

        return Ok();
    }
}