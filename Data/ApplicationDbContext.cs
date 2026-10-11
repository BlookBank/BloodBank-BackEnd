using Microsoft.EntityFrameworkCore;
using MobileBackEnd.Models;

namespace MobileBackEnd.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> Users => Set<UserProfile>();
    public DbSet<BloodRequest> BloodRequests => Set<BloodRequest>();
    public DbSet<Donor> Donors => Set<Donor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<BloodRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BloodGroup);
            entity.HasIndex(e => e.Status);
        });

        modelBuilder.Entity<Donor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BloodGroup);
        });
    }
}
