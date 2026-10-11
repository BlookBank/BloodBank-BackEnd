using Microsoft.EntityFrameworkCore;
using MobileBackEnd.Data;
using MobileBackEnd.Models;

namespace MobileBackEnd.Repositories;

public interface IBloodRequestRepository
{
    Task<IEnumerable<BloodRequest>> GetAllAsync(string? bloodGroup = null, string? status = null);
    Task<BloodRequest?> GetByIdAsync(int id);
    Task<BloodRequest> AddAsync(BloodRequest request);
    Task<bool> UpdateAsync(BloodRequest request);
    Task<bool> DeleteAsync(int id);
}

public class BloodRequestRepository : IBloodRequestRepository
{
    private readonly ApplicationDbContext _context;

    public BloodRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BloodRequest>> GetAllAsync(string? bloodGroup = null, string? status = null)
    {
        var query = _context.BloodRequests.AsQueryable();

        if (!string.IsNullOrWhiteSpace(bloodGroup) && !bloodGroup.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(r => r.BloodGroup.ToLower() == bloodGroup.Trim().ToLower());
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.Status.ToLower() == status.Trim().ToLower());
        }

        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<BloodRequest?> GetByIdAsync(int id)
    {
        return await _context.BloodRequests.FindAsync(id);
    }

    public async Task<BloodRequest> AddAsync(BloodRequest request)
    {
        _context.BloodRequests.Add(request);
        await _context.SaveChangesAsync();
        return request;
    }

    public async Task<bool> UpdateAsync(BloodRequest request)
    {
        _context.Entry(request).State = EntityState.Modified;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var request = await _context.BloodRequests.FindAsync(id);
        if (request == null) return false;

        _context.BloodRequests.Remove(request);
        return await _context.SaveChangesAsync() > 0;
    }
}
