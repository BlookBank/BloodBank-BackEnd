using MobileBackEnd.DTOs;
using MobileBackEnd.Models;
using MobileBackEnd.Repositories;

namespace MobileBackEnd.Services;

public interface IUserService
{
    Task<UserProfileDto?> GetUserByIdAsync(string id);
    Task<UserProfileDto> SyncUserAsync(SyncUserDto dto);
    Task<UserProfileDto?> UpdateUserAsync(string id, UpdateUserProfileDto dto);
}

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

    public async Task<UserProfileDto> SyncUserAsync(SyncUserDto dto)
    {
        var entity = new UserProfile
        {
            Id = dto.Id,
            Email = dto.Email,
            Name = dto.Name,
            PhoneNumber = dto.PhoneNumber,
            Gender = dto.Gender,
            BloodGroup = dto.BloodGroup,
            Location = dto.Location,
            CreatedAt = DateTime.UtcNow
        };

        var saved = await _repository.AddOrUpdateAsync(entity);
        return MapToDto(saved);
    }

    public async Task<UserProfileDto?> UpdateUserAsync(string id, UpdateUserProfileDto dto)
    {
        var user = await _repository.GetByIdAsync(id);
        if (user == null) return null;

        if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(dto.Gender)) user.Gender = dto.Gender;
        if (!string.IsNullOrWhiteSpace(dto.BloodGroup)) user.BloodGroup = dto.BloodGroup;
        if (!string.IsNullOrWhiteSpace(dto.Location)) user.Location = dto.Location;

        await _repository.UpdateAsync(user);
        return MapToDto(user);
    }

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
