using System.ComponentModel.DataAnnotations;

namespace MobileBackEnd.DTOs;

/// <summary>Perfil de usuario que el API devuelve al cliente.</summary>
public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Datos que envía la app después de registrarse en Firebase para crear (o actualizar) al usuario
/// en SQL Server. El Id de la fila siempre se toma del token, nunca de este objeto.
/// </summary>
public class SyncUserDto
{
    /// <summary>Se ignora: el id real sale del token de Firebase.</summary>
    public string? Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [MaxLength(10)]
    public string? BloodGroup { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }
}

/// <summary>
/// Cambios parciales sobre el perfil: los campos que lleguen vacios o en null no se modifican.
/// </summary>
public class UpdateUserProfileDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(20)]
    public string? Gender { get; set; }

    [MaxLength(10)]
    public string? BloodGroup { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }
}
