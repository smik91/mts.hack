using Microsoft.EntityFrameworkCore;
using Midiot.Data.Entities;
using Midiot.Infrastructure.Entities;

namespace Midiot.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<ConfirmationCodeEntity> ConfirmationCodes { get; set; }
}
