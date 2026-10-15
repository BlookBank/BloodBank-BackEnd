using MobileBackEnd.DTOs;
using MobileBackEnd.Models;
using MobileBackEnd.Repositories;

namespace MobileBackEnd.Services;

/// <summary>Reglas de negocio de las solicitudes de sangre.</summary>
public interface IBloodRequestService
{
    /// <summary>Lista todas las solicitudes; se puede filtrar por grupo sanguíneo y estado.</summary>
    Task<IEnumerable<BloodRequestDto>> GetRequestsAsync(string? bloodGroup = null, string? status = null);
    /// <summary>Lista las solicitudes publicadas por el usuario <paramref name="uid"/>.</summary>
    Task<IEnumerable<BloodRequestDto>> GetMyRequestsAsync(string uid);
    /// <summary>Devuelve una solicitud o null si no existe.</summary>
    Task<BloodRequestDto?> GetRequestByIdAsync(int id);
    /// <summary>Publica una solicitud a nombre del usuario <paramref name="uid"/> (siempre queda Active).</summary>
    Task<BloodRequestDto> CreateRequestAsync(string uid, CreateBloodRequestDto dto);
    /// <summary>Edita una solicitud; solo su dueño puede hacerlo.</summary>
    Task<OperationResult<BloodRequestDto>> UpdateRequestAsync(string uid, int id, CreateBloodRequestDto dto);
    /// <summary>Cambia el estado de una solicitud (Active, Fulfilled, Cancelled); solo su dueño.</summary>
    Task<OperationResult<BloodRequestDto>> UpdateStatusAsync(string uid, int id, string newStatus);
    /// <summary>Elimina una solicitud; solo su dueño puede hacerlo.</summary>
    Task<OperationResult<bool>> DeleteRequestAsync(string uid, int id);
}

/// <summary>Implementación de <see cref="IBloodRequestService"/>: valida propiedad y estados y delega el acceso a datos al repositorio.</summary>
public class BloodRequestService : IBloodRequestService
{
    /// <summary>Únicos estados permitidos para una solicitud.</summary>
    public static readonly string[] ValidStatuses = { "Active", "Fulfilled", "Cancelled" };

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

    public async Task<IEnumerable<BloodRequestDto>> GetMyRequestsAsync(string uid)
    {
        var requests = await _repository.GetByRequesterAsync(uid);
        return requests.Select(MapToDto);
    }

    public async Task<BloodRequestDto?> GetRequestByIdAsync(int id)
    {
        var request = await _repository.GetByIdAsync(id);
        return request == null ? null : MapToDto(request);
    }

    public async Task<BloodRequestDto> CreateRequestAsync(string uid, CreateBloodRequestDto dto)
    {
        var entity = new BloodRequest
        {
            RequesterId = uid, // siempre el usuario autenticado, nunca el que diga el cliente
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };
        Apply(entity, dto);

        var created = await _repository.AddAsync(entity);
        return MapToDto(created);
    }

    public async Task<OperationResult<BloodRequestDto>> UpdateRequestAsync(string uid, int id, CreateBloodRequestDto dto)
    {
        var request = await _repository.GetByIdAsync(id);
        if (request == null) return OperationResult<BloodRequestDto>.NotFound();
        // Solo quien publicó la solicitud puede modificarla.
        if (request.RequesterId != uid) return OperationResult<BloodRequestDto>.Forbidden();

        Apply(request, dto);
        await _repository.UpdateAsync(request);
        return OperationResult<BloodRequestDto>.Ok(MapToDto(request));
    }

    public async Task<OperationResult<BloodRequestDto>> UpdateStatusAsync(string uid, int id, string newStatus)
    {
        // Acepta cualquier combinacion de mayúsculas (active, ACTIVE) y la normaliza al valor oficial.
        var status = ValidStatuses.FirstOrDefault(s => s.Equals(newStatus?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (status == null)
            return OperationResult<BloodRequestDto>.Invalid($"Status must be one of: {string.Join(", ", ValidStatuses)}.");

        var request = await _repository.GetByIdAsync(id);
        if (request == null) return OperationResult<BloodRequestDto>.NotFound();
        // Solo quien publicó la solicitud puede cambiarle el estado.
        if (request.RequesterId != uid) return OperationResult<BloodRequestDto>.Forbidden();

        request.Status = status;
        await _repository.UpdateAsync(request);
        return OperationResult<BloodRequestDto>.Ok(MapToDto(request));
    }

    public async Task<OperationResult<bool>> DeleteRequestAsync(string uid, int id)
    {
        var request = await _repository.GetByIdAsync(id);
        if (request == null) return OperationResult<bool>.NotFound();
        // Solo quien publicó la solicitud puede eliminarla.
        if (request.RequesterId != uid) return OperationResult<bool>.Forbidden();

        await _repository.DeleteAsync(id);
        return OperationResult<bool>.Ok(true);
    }

    /// <summary>Copia los datos del DTO a la entidad limpiando espacios y normalizando el grupo sanguíneo a mayúsculas.</summary>
    private static void Apply(BloodRequest entity, CreateBloodRequestDto dto)
    {
        entity.PatientName = dto.PatientName.Trim();
        entity.BloodGroup = dto.BloodGroup.ToUpper().Trim();
        entity.NumberOfBags = dto.NumberOfBags;
        entity.ContactNumber = dto.ContactNumber.Trim();
        entity.EmergencyContact = dto.EmergencyContact?.Trim();
        entity.CaseDescription = dto.CaseDescription.Trim();
        entity.Gender = dto.Gender;
        entity.NeedBeforeDate = dto.NeedBeforeDate;
        entity.Location = dto.Location.Trim();
        entity.Others = dto.Others?.Trim();
    }

    /// <summary>Convierte la entidad en el DTO que se envía al cliente.</summary>
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
