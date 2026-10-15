using MobileBackEnd.DTOs;
using MobileBackEnd.Models;
using MobileBackEnd.Repositories;

namespace MobileBackEnd.Services;

/// <summary>Reglas de negocio de los usuarios (perfil guardado en SQL Server).</summary>
public interface IUserService
{
    /// <summary>Devuelve un usuario por su UID de Firebase o null si no existe.</summary>
    Task<UserProfileDto?> GetUserByIdAsync(string id);
    /// <summary>Crea o actualiza en SQL Server al usuario autenticado en Firebase.</summary>
    Task<OperationResult<UserProfileDto>> SyncUserAsync(string uid, SyncUserDto dto);
    /// <summary>Aplica cambios parciales al perfil; devuelve null si el usuario no existe.</summary>
    Task<UserProfileDto?> UpdateUserAsync(string id, UpdateUserProfileDto dto);
}

/// <summary>Implementación de <see cref="IUserService"/>.</summary>
public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserProfileDto?> GetUserByIdAsync(string id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<OperationResult<UserProfileDto>> SyncUserAsync(string uid, SyncUserDto dto)
    {
        // El email es único: si ya lo usa otra cuenta se rechaza en vez de fallar con un 500
        var sameEmail = await _repository.GetByEmailAsync(dto.Email);
        if (sameEmail != null && sameEmail.Id != uid)
            return OperationResult<UserProfileDto>.Conflict("That email is already linked to another account.");

        // El Id es el UID del token, nunca el que mande el cliente.
        var entity = new UserProfile
        {
            Id = uid,
            Email = dto.Email.Trim(),
            Name = dto.Name.Trim(),
            PhoneNumber = dto.PhoneNumber,
            Gender = dto.Gender,
            BloodGroup = dto.BloodGroup,
            Location = dto.Location,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await _repository.AddOrUpdateAsync(entity);
        return OperationResult<UserProfileDto>.Ok(MapToDto(saved));
    }

    public async Task<UserProfileDto?> UpdateUserAsync(string id, UpdateUserProfileDto dto)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null) return null;

        // Solo se cambian los campos que llegan con valor (actualizacion parcial).
        if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name.Trim();
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(dto.Gender)) user.Gender = dto.Gender;
        if (!string.IsNullOrWhiteSpace(dto.BloodGroup)) user.BloodGroup = dto.BloodGroup;
        if (!string.IsNullOrWhiteSpace(dto.Location)) user.Location = dto.Location;

        await _repository.UpdateAsync(user);
        return MapToDto(user);
    }

    /// <summary>Convierte la entidad en el DTO que se envía al cliente.</summary>
    private static UserProfileDto MapToDto(UserProfile u) => new()
    {
        Id = u.Id,
        Email = u.Email,
        Name = u.Name,
        PhoneNumber = u.PhoneNumber,
        Gender = u.Gender,
        BloodGroup = u.BloodGroup,
        Location = u.Location,
        CreatedAt = u.CreatedAt
    };
}
