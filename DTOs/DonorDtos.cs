using System.ComponentModel.DataAnnotations;

namespace MobileBackEnd.DTOs;

public class DonorDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactNumber { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime? LastDonationDate { get; set; }
    public string? LastDonationDateFormatted => LastDonationDate?.ToString("dd MMMM yyyy");
    public string? PictureUrl { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}

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
