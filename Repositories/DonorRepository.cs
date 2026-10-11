using Microsoft.EntityFrameworkCore;
using MobileBackEnd.Data;
using MobileBackEnd.Models;

namespace MobileBackEnd.Repositories;

public interface IDonorRepository
{
    Task<IEnumerable<Donor>> GetAllAsync(string? bloodGroup = null, bool? isAvailable = null);
    Task<Donor?> GetByIdAsync(int id);
    Task<Donor?> GetByUserIdAsync(string userId);
    Task<Donor> AddAsync(Donor donor);
    Task<bool> UpdateAsync(Donor donor);
}

public class DonorRepository : IDonorRepository
{
    private readonly ApplicationDbContext _context;

    public DonorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Donor>> GetAllAsync(string? bloodGroup = null, bool? isAvailable = null)
    {
        var query = _context.Donors.AsQueryable();

        if (!string.IsNullOrWhiteSpace(bloodGroup) && !bloodGroup.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(d => d.BloodGroup.ToLower() == bloodGroup.Trim().ToLower());
        }

        if (isAvailable.HasValue)
        {
            query = query.Where(d => d.IsAvailable == isAvailable.Value);
        }

        return await query.OrderByDescending(d => d.CreatedAt).ToListAsync();
    }

    public async Task<Donor?> GetByIdAsync(int id)
    {
        return await _context.Donors.FindAsync(id);
    }

    public async Task<Donor?> GetByUserIdAsync(string userId)
    {
        return await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<Donor> AddAsync(Donor donor)
    {
        _context.Donors.Add(donor);
        await _context.SaveChangesAsync();
        return donor;
    }

    public async Task<bool> UpdateAsync(Donor donor)
    {
        _context.Entry(donor).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }
}
