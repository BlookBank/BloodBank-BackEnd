using Microsoft.EntityFrameworkCore;
using MobileBackEnd.Data;
using MobileBackEnd.Models;

namespace MobileBackEnd.Repositories;

/// <summary>Acceso a datos de la tabla Donors.</summary>
public interface IDonorRepository
{
    /// <summary>Lista donantes, opcionalmente filtrados por grupo sanguíneo y disponibilidad.</summary>
    Task<IEnumerable<Donor>> GetAllAsync(string? bloodGroup = null, bool? isAvailable = null);
    /// <summary>Busca un donante por su id.</summary>
    Task<Donor?> GetByIdAsync(int id);
    /// <summary>Busca el registro de donante de un usuario (por su UID de Firebase).</summary>
    Task<Donor?> GetByUserIdAsync(string userId);
    /// <summary>Inserta un donante nuevo y devuelve la entidad con su id.</summary>
    Task<Donor> AddAsync(Donor donor);
    /// <summary>Guarda los cambios de un donante.</summary>
    Task<bool> UpdateAsync(Donor donor);
    /// <summary>Elimina un donante; false si no existe.</summary>
    Task<bool> DeleteAsync(int id);
}

/// <summary>Implementación con Entity Framework Core de <see cref="IDonorRepository"/>.</summary>
public class DonorRepository : IDonorRepository
{
    private readonly ApplicationDbContext _context;

    public DonorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Donor>> GetAllAsync(string? bloodGroup = null, bool? isAvailable = null)
    {
        // AsNoTracking: son consultas de solo lectura, EF no necesita vigilar los cambios (más rápido).
        var query = _context.Donors.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(bloodGroup) && !bloodGroup.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            // "All" o vacio significa sin filtro; el grupo se compara en mayúsculas porque así se guarda.
            var group = bloodGroup.Trim().ToUpper();
            query = query.Where(d => d.BloodGroup == group);
        }

        if (isAvailable.HasValue)
        {
            query = query.Where(d => d.IsAvailable == isAvailable.Value);
        }

        // Los más recientes primero.
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

    public async Task<bool> DeleteAsync(int id)
    {
        var donor = await _context.Donors.FindAsync(id);
        if (donor == null) return false;

        _context.Donors.Remove(donor);
        return await _context.SaveChangesAsync() > 0;
    }
}
