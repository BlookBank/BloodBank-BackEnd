using MobileBackEnd.DTOs;
using MobileBackEnd.Models;
using MobileBackEnd.Repositories;

namespace MobileBackEnd.Services;

public interface IBloodRequestService
{
    Task<IEnumerable<BloodRequestDto>> GetRequestsAsync(string? bloodGroup = null, string? status = null);
    Task<BloodRequestDto?> GetRequestByIdAsync(int id);
    Task<BloodRequestDto> CreateRequestAsync(CreateBloodRequestDto dto);
    Task<bool> UpdateStatusAsync(int id, string newStatus);
}

public class BloodRequestService : IBloodRequestService
{
    private readonly IBloodRequestRepository _repository;

    public BloodRequestService(IBloodRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<BloodRequestDto>> GetRequestsAsync(string? bloodGroup = null, string? status = null)
    {
        var requests = await _repository.GetAllAsync(bloodGroup, status);
        return requests.Select(MapToDto);
    }

    public async Task<BloodRequestDto?> GetRequestByIdAsync(int id)
    {
        var request = await _repository.GetByIdAsync(id);
        return request == null ? null : MapToDto(request);
    }

    public async Task<BloodRequestDto> CreateRequestAsync(CreateBloodRequestDto dto)
    {
        var entity = new BloodRequest
        {
            RequesterId = dto.RequesterId,
            PatientName = dto.PatientName,
            BloodGroup = dto.BloodGroup.ToUpper().Trim(),
            NumberOfBags = dto.NumberOfBags,
            ContactNumber = dto.ContactNumber,
            EmergencyContact = dto.EmergencyContact,
            CaseDescription = dto.CaseDescription,
            Gender = dto.Gender,
            NeedBeforeDate = dto.NeedBeforeDate,
            Location = dto.Location,
            Others = dto.Others,
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<bool> UpdateStatusAsync(int id, string newStatus)
    {
        var request = await _repository.GetByIdAsync(id);
        if (request == null) return false;

        request.Status = newStatus;
        return await _repository.UpdateAsync(request);
    }

    private static BloodRequestDto MapToDto(BloodRequest r) => new()
    {
        Id = r.Id,
        RequesterId = r.RequesterId,
        PatientName = r.PatientName,
        BloodGroup = r.BloodGroup,
        NumberOfBags = r.NumberOfBags,
        ContactNumber = r.ContactNumber,
        EmergencyContact = r.EmergencyContact,
        CaseDescription = r.CaseDescription,
        Gender = r.Gender,
        NeedBeforeDate = r.NeedBeforeDate,
        Location = r.Location,
        Others = r.Others,
        Status = r.Status,
        CreatedAt = r.CreatedAt
    };
}
