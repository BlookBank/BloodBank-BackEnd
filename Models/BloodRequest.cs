using System.ComponentModel.DataAnnotations;

namespace MobileBackEnd.Models;

public class BloodRequest
{
    [Key]
    public int Id { get; set; }

    public string? RequesterId { get; set; }

    [Required]
    [MaxLength(100)]
    public string PatientName { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string BloodGroup { get; set; } = string.Empty;

    [Range(1, 50)]
    public int NumberOfBags { get; set; } = 1;

    [Required]
    [MaxLength(20)]
    public string ContactNumber { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? EmergencyContact { get; set; }

    [Required]
    [MaxLength(200)]
    public string CaseDescription { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Gender { get; set; }

    public DateTime NeedBeforeDate { get; set; }

    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Others { get; set; }

    [MaxLength(20)]
    public string Status { get; set; } = "Active"; // Active, Fulfilled, Cancelled

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
