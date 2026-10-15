using System.ComponentModel.DataAnnotations;

namespace MobileBackEnd.Models;

public class UserProfile
{
    [Key]
    [MaxLength(128)]
    public string Id { get; set; } = string.Empty; // Firebase UID

    [Required]
    [EmailAddress]
    [MaxLength(256)]
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

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
