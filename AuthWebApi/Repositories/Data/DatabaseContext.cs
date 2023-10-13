using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Repositories.Data;
public class DatabaseContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RefreshToken>()
            .HasOne<User>()
            .WithMany();

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
}