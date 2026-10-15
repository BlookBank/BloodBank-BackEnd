using System.ComponentModel.DataAnnotations;

namespace MobileBackEnd.DTOs;

/// <summary>Donante tal como se devuelve al cliente (incluye la fecha ya formateada).</summary>
public class DonorDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime? LastDonationDate { get; set; }
    /// <summary>Fecha de la última donación lista para mostrar, por ejemplo "10 February 2024".</summary>
    public string? LastDonationDateFormatted => LastDonationDate?.ToString("dd MMMM yyyy");
    public string? PictureUrl { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Datos para registrarse como donante (o actualizar el registro). El UserId se ignora:
/// el API usa el UID del usuario autenticado.
/// </summary>
public class CreateDonorDto
{
    public string? UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ContactNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string BloodGroup { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public DateTime? LastDonationDate { get; set; }

    [MaxLength(500)]
    public string? PictureUrl { get; set; }
}

/// <summary>Cuerpo del PATCH que activa o desactiva la disponibilidad del donante.</summary>
public class UpdateAvailabilityDto
{
    public bool IsAvailable { get; set; }
}
