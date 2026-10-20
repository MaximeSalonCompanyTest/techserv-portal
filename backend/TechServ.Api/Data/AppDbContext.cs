using Microsoft.EntityFrameworkCore;
using TechServ.Api.Models;

namespace TechServ.Api.Data;

// The bridge between C# and the database: it opens connections, turns LINQ into SQL and saves changes.
public class AppDbContext : DbContext
{
    // The options (Postgres + connection string) come from AddDbContext in Program.cs, through dependency injection.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // One DbSet per table. A query like _db.Users.Where(...) becomes a SELECT on the Users table.
    public DbSet<User> Users => Set<User>();
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<StatusChange> StatusChanges => Set<StatusChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Last safeguard against duplicate emails, even if two sign-ups arrive at once.
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        // Enums are saved as text so the database stays readable ("Open" instead of 0).
        modelBuilder.Entity<User>().Property(u => u.Role).HasConversion<string>();
        modelBuilder.Entity<ServiceRequest>().Property(r => r.Category).HasConversion<string>();
        modelBuilder.Entity<ServiceRequest>().Property(r => r.Priority).HasConversion<string>();
        modelBuilder.Entity<ServiceRequest>().Property(r => r.Status).HasConversion<string>();
        modelBuilder.Entity<StatusChange>().Property(s => s.FromStatus).HasConversion<string>();
        modelBuilder.Entity<StatusChange>().Property(s => s.ToStatus).HasConversion<string>();
    }
}
