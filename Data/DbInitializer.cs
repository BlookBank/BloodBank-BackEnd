using Microsoft.EntityFrameworkCore;
using MobileBackEnd.Models;

namespace MobileBackEnd.Data;

/// <summary>
/// Prepara la base de datos al arrancar: aplica las migraciones pendientes y, solo en desarrollo,
/// carga solicitudes y donantes de ejemplo para que las listas no aparezcan vacias.
/// </summary>
public static class DbInitializer
{
    /// <summary>Crea/actualiza el esquema y, si seedSampleData es true, inserta datos de ejemplo una sola vez.</summary>
    /// <param name="context">Contexto de la base de datos.</param>
    /// <param name="seedSampleData">true para insertar datos de ejemplo (solo en Development).</param>
    public static async Task InitializeAsync(ApplicationDbContext context, bool seedSampleData)
    {
        // Aplica las migraciones (carpeta Migrations) que falten en SQL Server.
        await context.Database.MigrateAsync();

        if (!seedSampleData) return;

        // Solo se insertan si la tabla esta vacia, así no se duplican en cada arranque.
        if (!context.BloodRequests.Any())
        {
            context.BloodRequests.AddRange(
                new BloodRequest
                {
                    PatientName = "Jorge Perez",
                    BloodGroup = "A+",
                    NumberOfBags = 2,
                    ContactNumber = "+18092249304",
                    EmergencyContact = "+18095559999",
                    CaseDescription = "Accident",
                    Gender = "Male",
                    NeedBeforeDate = DateTime.UtcNow.AddDays(2),
                    Location = "Santo Domingo, DN",
                    Others = "Need very urgent",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5)
                },
                new BloodRequest
                {
                    PatientName = "Elena Gomez",
                    BloodGroup = "O+",
                    NumberOfBags = 3,
                    ContactNumber = "+18293344556",
                    EmergencyContact = "+18293344557",
                    CaseDescription = "Surgery",
                    Gender = "Female",
                    NeedBeforeDate = DateTime.UtcNow.AddDays(3),
                    Location = "Santo Domingo Este, RD",
                    Others = "Hospital Plaza de la Salud",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-45)
                },
                new BloodRequest
                {
                    PatientName = "Carlos Rodriguez",
                    BloodGroup = "B+",
                    NumberOfBags = 1,
                    ContactNumber = "+18097788990",
                    EmergencyContact = "+18097788991",
                    CaseDescription = "Cancer Patient",
                    Gender = "Male",
                    NeedBeforeDate = DateTime.UtcNow.AddDays(5),
                    Location = "Santiago, RD",
                    Others = "Treatment session donation",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddHours(-3)
                },
                new BloodRequest
                {
                    PatientName = "Ana Martinez",
                    BloodGroup = "AB+",
                    NumberOfBags = 2,
                    ContactNumber = "+18094433221",
                    EmergencyContact = "+18094433222",
                    CaseDescription = "Leukemia Treatment",
                    Gender = "Female",
                    NeedBeforeDate = DateTime.UtcNow.AddDays(4),
                    Location = "La Vega, RD",
                    Others = "Cedimat, urgente",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddHours(-8)
                },
                new BloodRequest
                {
                    PatientName = "Pedro Almonte",
                    BloodGroup = "O-",
                    NumberOfBags = 4,
                    ContactNumber = "+18091122334",
                    EmergencyContact = "+18091122335",
                    CaseDescription = "Emergency Surgery",
                    Gender = "Male",
                    NeedBeforeDate = DateTime.UtcNow.AddDays(1),
                    Location = "Santo Domingo, DN",
                    Others = "Universal donor needed urgently",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            );
        }

        // Donantes de ejemplo (sin UserId: no pertenecen a ninguna cuenta real).
        if (!context.Donors.Any())
        {
            context.Donors.AddRange(
                new Donor
                {
                    Name = "Jorge Perez",
                    ContactNumber = "+18092249304",
                    BloodGroup = "B+",
                    Location = "Santo Domingo, RD",
                    LastDonationDate = DateTime.UtcNow.AddMonths(-4),
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-20)
                },
                new Donor
                {
                    Name = "Maria Santos",
                    ContactNumber = "+18095551234",
                    BloodGroup = "O+",
                    Location = "Santiago, RD",
                    LastDonationDate = DateTime.UtcNow.AddMonths(-6),
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new Donor
                {
                    Name = "Luis Fernandez",
                    ContactNumber = "+18096677889",
                    BloodGroup = "A+",
                    Location = "Santo Domingo, RD",
                    LastDonationDate = DateTime.UtcNow.AddMonths(-3),
                    IsAvailable = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                }
            );
        }

        await context.SaveChangesAsync();
    }
}
