using MobileBackEnd.DTOs;
using MobileBackEnd.Models;
using MobileBackEnd.Repositories;

namespace MobileBackEnd.Services;

public interface IDonorService
{
    Task<IEnumerable<DonorDto>> GetDonorsAsync(string? bloodGroup = null, bool? isAvailable = null);
    Task<DonorDto?> GetDonorByIdAsync(int id);
    Task<DonorDto?> GetDonorByUserIdAsync(string userId);
    Task<DonorDto> RegisterDonorAsync(string uid, CreateDonorDto dto);
    Task<OperationResult<DonorDto>> UpdateDonorAsync(string uid, int id, CreateDonorDto dto);
    Task<OperationResult<DonorDto>> SetAvailabilityAsync(string uid, int id, bool isAvailable);
    Task<OperationResult<bool>> DeleteDonorAsync(string uid, int id);
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

    /// <summary>Registra al usuario como donante; si ya lo estaba, actualiza su registro.</summary>
    public async Task<DonorDto> RegisterDonorAsync(string uid, CreateDonorDto dto)
    {
        var existing = await _repository.GetByUserIdAsync(uid);
        if (existing != null)
        {
            Apply(existing, dto);
            await _repository.UpdateAsync(existing);
            return MapToDto(existing);
        }

        var entity = new Donor
        {
            UserId = uid,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };
        Apply(entity, dto);

        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<OperationResult<DonorDto>> UpdateDonorAsync(string uid, int id, CreateDonorDto dto)
    {
        var donor = await _repository.GetByIdAsync(id);
        if (donor == null) return OperationResult<DonorDto>.NotFound();
        if (donor.UserId != uid) return OperationResult<DonorDto>.Forbidden();

        Apply(donor, dto);
        await _repository.UpdateAsync(donor);
        return OperationResult<DonorDto>.Ok(MapToDto(donor));
    }

    public async Task<OperationResult<DonorDto>> SetAvailabilityAsync(string uid, int id, bool isAvailable)
    {
        var donor = await _repository.GetByIdAsync(id);
        if (donor == null) return OperationResult<DonorDto>.NotFound();
        if (donor.UserId != uid) return OperationResult<DonorDto>.Forbidden();

        donor.IsAvailable = isAvailable;
        await _repository.UpdateAsync(donor);
        return OperationResult<DonorDto>.Ok(MapToDto(donor));
    }

    public async Task<OperationResult<bool>> DeleteDonorAsync(string uid, int id)
    {
        var donor = await _repository.GetByIdAsync(id);
        if (donor == null) return OperationResult<bool>.NotFound();
        if (donor.UserId != uid) return OperationResult<bool>.Forbidden();

        await _repository.DeleteAsync(id);
        return OperationResult<bool>.Ok(true);
    }

    private static void Apply(Donor entity, CreateDonorDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.ContactNumber = dto.ContactNumber.Trim();
        entity.BloodGroup = dto.BloodGroup.ToUpper().Trim();
        entity.Location = dto.Location.Trim();
        entity.LastDonationDate = dto.LastDonationDate;
        entity.PictureUrl = dto.PictureUrl;
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
