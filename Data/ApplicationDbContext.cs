using MarketingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketingAPI.Data;

/// <summary>
/// 
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    /// <summary>
    /// The Users table.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// The Customers table.
    /// </summary>
    public DbSet<Customer> Customers { get; set; }

    /// <summary>
    /// The Occasions table.
    /// </summary>
    public DbSet<Occasion> Occasions { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique(); // Ensure login username and password are unique
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique(); // Ensure email is unique.
        });
    }
}