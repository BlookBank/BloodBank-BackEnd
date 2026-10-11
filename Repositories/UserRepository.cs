using Microsoft.EntityFrameworkCore;
using MobileBackEnd.Data;
using MobileBackEnd.Models;

namespace MobileBackEnd.Repositories;

public interface IUserRepository
{
    Task<UserProfile?> GetByIdAsync(string id);
    Task<UserProfile?> GetByEmailAsync(string email);
    Task<UserProfile> AddOrUpdateAsync(UserProfile user);
    Task<bool> UpdateAsync(UserProfile user);
}

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
