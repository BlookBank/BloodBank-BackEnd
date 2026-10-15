using System.ComponentModel.DataAnnotations;

namespace MobileBackEnd.DTOs;

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
