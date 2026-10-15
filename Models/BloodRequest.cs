using System.ComponentModel.DataAnnotations;

namespace MobileBackEnd.Models;

/// <summary>
/// Solicitud de sangre publicada por un usuario para un paciente (tabla BloodRequests).
/// </summary>
public class BloodRequest
{
    /// <summary>Clave primaria autoincremental.</summary>
    [Key]
    public int Id { get; set; }

    /// <summary>UID de Firebase de quien publicó la solicitud; solo esa persona puede modificarla.</summary>
    [MaxLength(128)]
    public string? RequesterId { get; set; } // Firebase UID

    /// <summary>Nombre del paciente que necesita la sangre.</summary>
    [Required]
    [MaxLength(100)]
    public string PatientName { get; set; } = string.Empty;

    /// <summary>Grupo sanguíneo requerido, en mayúsculas.</summary>
    [Required]
    [MaxLength(10)]
    public string BloodGroup { get; set; } = string.Empty;

    /// <summary>Cantidad de bolsas necesarias (de 1 a 50).</summary>
    [Range(1, 50)]
    public int NumberOfBags { get; set; } = 1;

    /// <summary>Teléfono principal de contacto.</summary>
    [Required]
    [MaxLength(20)]
    public string ContactNumber { get; set; } = string.Empty;

    /// <summary>Teléfono alterno para emergencias (opcional).</summary>
    [MaxLength(20)]
    public string? EmergencyContact { get; set; }

    /// <summary>Motivo o caso clínico (cirugía, accidente, etc.).</summary>
    [Required]
    [MaxLength(200)]
    public string CaseDescription { get; set; } = string.Empty;

    /// <summary>Género del paciente (opcional).</summary>
    [MaxLength(20)]
    public string? Gender { get; set; }

    /// <summary>Fecha límite en la que se necesita la sangre.</summary>
    public DateTime NeedBeforeDate { get; set; }

    /// <summary>Hospital, ciudad o zona donde se necesita la donación.</summary>
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    /// <summary>Notas adicionales (opcional).</summary>
    [MaxLength(500)]
    public string? Others { get; set; }

    /// <summary>Estado de la solicitud: Active, Fulfilled o Cancelled.</summary>
    [MaxLength(20)]
    public string Status { get; set; } = "Active"; // Active, Fulfilled, Cancelled

    /// <summary>Fecha de publicación en UTC; se usa para ordenar y calcular "hace X minutos".</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
