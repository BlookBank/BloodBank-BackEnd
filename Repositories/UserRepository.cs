using Microsoft.EntityFrameworkCore;
using MobileBackEnd.Data;
using MobileBackEnd.Models;

namespace MobileBackEnd.Repositories;

/// <summary>Acceso a datos de la tabla Users.</summary>
public interface IUserRepository
{
    /// <summary>Busca un usuario por su UID de Firebase.</summary>
    Task<UserProfile?> GetByIdAsync(string id);
    /// <summary>Busca un usuario por correo (sin distinguir mayúsculas).</summary>
    Task<UserProfile?> GetByEmailAsync(string email);
    /// <summary>Inserta el usuario si no existe; si existe, actualiza sus datos.</summary>
    Task<UserProfile> AddOrUpdateAsync(UserProfile user);
    /// <summary>Guarda los cambios de un usuario ya existente.</summary>
    Task<bool> UpdateAsync(UserProfile user);
}

/// <summary>Implementación con Entity Framework Core de <see cref="IUserRepository"/>.</summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetByIdAsync(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<UserProfile?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower());
    }

    public async Task<UserProfile> AddOrUpdateAsync(UserProfile user)
    {
        var existing = await _context.Users.FindAsync(user.Id);
        if (existing == null)
        {
            _context.Users.Add(user);
        }
        else
        {
            // Nombre y correo siempre se actualizan; los demas campos solo si llegan con valor,
            // para no borrar datos al sincronizar desde el registro.
            existing.Name = user.Name;
            existing.Email = user.Email;
            if (!string.IsNullOrEmpty(user.PhoneNumber)) existing.PhoneNumber = user.PhoneNumber;
            if (!string.IsNullOrEmpty(user.Gender)) existing.Gender = user.Gender;
            if (!string.IsNullOrEmpty(user.BloodGroup)) existing.BloodGroup = user.BloodGroup;
            if (!string.IsNullOrEmpty(user.Location)) existing.Location = user.Location;
            existing.UpdatedAt = DateTime.UtcNow;
            _context.Entry(existing).State = EntityState.Modified;
            user = existing;
        }

        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(UserProfile user)
    {
        user.UpdatedAt = DateTime.UtcNow;
        _context.Entry(user).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }
}
