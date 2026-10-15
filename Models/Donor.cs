using System.ComponentModel.DataAnnotations;

namespace MobileBackEnd.Models;

/// <summary>
/// Persona que se registro para donar sangre (tabla Donors en SQL Server).
/// Un usuario autenticado solo puede tener un registro de donante (índice único en UserId).
/// </summary>
public class Donor
{
    /// <summary>Clave primaria autoincremental.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>UID de Firebase del dueño del registro; null en los donantes de ejemplo.</summary>
    [MaxLength(128)]
    public string? UserId { get; set; } // Firebase UID

    /// <summary>Nombre del donante.</summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Teléfono para contactar al donante.</summary>
    [Required]
    [MaxLength(20)]
    public string ContactNumber { get; set; } = string.Empty;

    /// <summary>Grupo sanguíneo normalizado en mayúsculas (A+, O-, ...).</summary>
    [Required]
    [MaxLength(10)]
    public string BloodGroup { get; set; } = string.Empty;

    /// <summary>Ciudad o zona donde vive el donante.</summary>
    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    /// <summary>Fecha de la última donación, si la hay.</summary>
    public DateTime? LastDonationDate { get; set; }

    /// <summary>URL de la foto del donante (opcional).</summary>
    [MaxLength(500)]
    public string? PictureUrl { get; set; }

    /// <summary>Indica si el donante puede ser contactado ahora mismo.</summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>Fecha de registro en UTC.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
