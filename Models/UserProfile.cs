using System.ComponentModel.DataAnnotations;

namespace MobileBackEnd.Models;

/// <summary>
/// Perfil de un usuario de la app (tabla Users en SQL Server).
/// El Id es el UID que asigna Firebase Authentication al registrarse, así que un usuario
/// de Firebase y su fila en esta tabla siempre comparten el mismo identificador.
/// </summary>
public class UserProfile
{
    /// <summary>UID de Firebase del usuario (clave primaria).</summary>
    [Key]
    [MaxLength(128)]
    public string Id { get; set; } = string.Empty; // Firebase UID

    /// <summary>Correo del usuario. Es único en la tabla (índice único en el DbContext).</summary>
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Nombre completo mostrado en la app.</summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Teléfono de contacto (opcional).</summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>Género del usuario: male o female (opcional).</summary>
    [MaxLength(20)]
    public string? Gender { get; set; }

    /// <summary>Grupo sanguíneo, por ejemplo A+ u O- (opcional).</summary>
    [MaxLength(10)]
    public string? BloodGroup { get; set; }

    /// <summary>Ciudad o zona del usuario (opcional).</summary>
    [MaxLength(200)]
    public string? Location { get; set; }

    /// <summary>Fecha de creación en UTC.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>Fecha de la última modificación en UTC; null si nunca se ha modificado.</summary>
    public DateTime? UpdatedAt { get; set; }
}
