using System.ComponentModel.DataAnnotations;

namespace MobileBackEnd.DTOs;

public class BloodRequestDto
{
    public int Id { get; set; }
    public string? RequesterId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string BloodGroup { get; set; } = string.Empty;
    public int NumberOfBags { get; set; }
    public string ContactNumber { get; set; } = string.Empty;
    public string? EmergencyContact { get; set; }
    public string CaseDescription { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public DateTime NeedBeforeDate { get; set; }
    public string NeedBeforeDateFormatted => NeedBeforeDate.ToString("dd MMMM yyyy");
    public string Location { get; set; } = string.Empty;
    public string? Others { get; set; }
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; }
    public string TimeAgo
    {
        get
        {
            var span = DateTime.UtcNow - CreatedAt;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} minute ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";
            return $"{(int)span.TotalDays} days ago";
        }
    }
}

public class CreateBloodRequestDto
{
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

    [Required]
    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Others { get; set; }
}
