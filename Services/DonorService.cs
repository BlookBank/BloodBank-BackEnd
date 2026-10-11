using MobileBackEnd.DTOs;
using MobileBackEnd.Models;
using MobileBackEnd.Repositories;

namespace MobileBackEnd.Services;

public interface IDonorService
{
    Task<IEnumerable<DonorDto>> GetDonorsAsync(string? bloodGroup = null, bool? isAvailable = null);
    Task<DonorDto?> GetDonorByIdAsync(int id);
    Task<DonorDto?> GetDonorByUserIdAsync(string userId);
    Task<DonorDto> RegisterDonorAsync(CreateDonorDto dto);
}

public class DonorService : IDonorService
{
    private readonly IDonorRepository _repository;

    public DonorService(IDonorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DonorDto>> GetDonorsAsync(string? bloodGroup = null, bool? isAvailable = null)
    {
        var donors = await _repository.GetAllAsync(bloodGroup, isAvailable);
        return donors.Select(MapToDto);
    }

    public async Task<DonorDto?> GetDonorByIdAsync(int id)
    {
        var donor = await _repository.GetByIdAsync(id);
        return donor == null ? null : MapToDto(donor);
    }

    public async Task<DonorDto?> GetDonorByUserIdAsync(string userId)
    {
        var donor = await _repository.GetByUserIdAsync(userId);
        return donor == null ? null : MapToDto(donor);
    }

    public async Task<DonorDto> RegisterDonorAsync(CreateDonorDto dto)
    {
        var entity = new Donor
        {
            UserId = dto.UserId,
            Name = dto.Name,
            ContactNumber = dto.ContactNumber,
            BloodGroup = dto.BloodGroup.ToUpper().Trim(),
            Location = dto.Location,
            LastDonationDate = dto.LastDonationDate,
            PictureUrl = dto.PictureUrl,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    private static DonorDto MapToDto(Donor d) => new()
    {
        Id = d.Id,
        UserId = d.UserId,
        Name = d.Name,
        ContactNumber = d.ContactNumber,
        BloodGroup = d.BloodGroup,
        Location = d.Location,
        LastDonationDate = d.LastDonationDate,
        PictureUrl = d.PictureUrl,
        IsAvailable = d.IsAvailable,
        CreatedAt = d.CreatedAt
    };
}
