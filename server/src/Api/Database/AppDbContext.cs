using Api.Database.Configurations;
using Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
}
