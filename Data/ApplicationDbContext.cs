using Microsoft.EntityFrameworkCore;
using MobileBackEnd.Models;

namespace MobileBackEnd.Data;

/// <summary>
/// Contexto de Entity Framework Core para SQL Server. Expone las tres tablas de la app:
/// Users, Donors y BloodRequests.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Tabla de usuarios (perfil sincronizado desde Firebase).
    public DbSet<UserProfile> Users => Set<UserProfile>();
    // Tabla de solicitudes de sangre.
    public DbSet<BloodRequest> BloodRequests => Set<BloodRequest>();
    // Tabla de donantes.
    public DbSet<Donor> Donors => Set<Donor>();

    // Configura claves e índices de cada tabla.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // El correo no se puede repetir entre usuarios.
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Índices para acelerar los filtros por grupo sanguíneo, estado y solicitante.
        modelBuilder.Entity<BloodRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BloodGroup);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.RequesterId);
        });

        // Índice por grupo sanguíneo (busqueda de donantes).
        modelBuilder.Entity<Donor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BloodGroup);
            // Un usuario solo puede tener un perfil de donante
            entity.HasIndex(e => e.UserId)
                  .IsUnique()
                  .HasFilter("[UserId] IS NOT NULL");
        });
    }
}
