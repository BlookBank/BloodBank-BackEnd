using Microsoft.EntityFrameworkCore;
using MobileBackEnd.Data;
using MobileBackEnd.Models;

namespace MobileBackEnd.Repositories;

/// <summary>Acceso a datos de la tabla BloodRequests.</summary>
public interface IBloodRequestRepository
{
    /// <summary>Lista solicitudes, opcionalmente filtradas por grupo sanguíneo y estado.</summary>
    Task<IEnumerable<BloodRequest>> GetAllAsync(string? bloodGroup = null, string? status = null);
    /// <summary>Lista las solicitudes publicadas por un usuario.</summary>
    Task<IEnumerable<BloodRequest>> GetByRequesterAsync(string requesterId);
    /// <summary>Busca una solicitud por su id.</summary>
    Task<BloodRequest?> GetByIdAsync(int id);
    /// <summary>Inserta una solicitud nueva y devuelve la entidad con su id.</summary>
    Task<BloodRequest> AddAsync(BloodRequest request);
    /// <summary>Guarda los cambios de una solicitud.</summary>
    Task<bool> UpdateAsync(BloodRequest request);
    /// <summary>Elimina una solicitud; false si no existe.</summary>
    Task<bool> DeleteAsync(int id);
}

/// <summary>Implementación con Entity Framework Core de <see cref="IBloodRequestRepository"/>.</summary>
public class BloodRequestRepository : IBloodRequestRepository
{
    private readonly ApplicationDbContext _context;

    public BloodRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BloodRequest>> GetAllAsync(string? bloodGroup = null, string? status = null)
    {
        // AsNoTracking: consulta de solo lectura (EF no vigila cambios).
        var query = _context.BloodRequests.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(bloodGroup) && !bloodGroup.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            // "All" o vacio significa sin filtro; el grupo se guarda en mayúsculas.
            var group = bloodGroup.Trim().ToUpper();
            query = query.Where(r => r.BloodGroup == group);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            // El estado se compara sin distinguir mayúsculas (active = Active).
            var normalized = status.Trim().ToLower();
            query = query.Where(r => r.Status.ToLower() == normalized);
        }

        // Las más recientes primero: es el orden del feed de la app.
        return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<BloodRequest>> GetByRequesterAsync(string requesterId)
    {
        return await _context.BloodRequests
            .AsNoTracking()
            .Where(r => r.RequesterId == requesterId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
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
