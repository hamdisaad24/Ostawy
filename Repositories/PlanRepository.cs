using Microsoft.EntityFrameworkCore;
using Ostawy.Data;
using Ostawy.Models;

namespace Ostawy.Repositories;

public class PlanRepository
{
    private readonly ApplicationDbContext _context;

    public PlanRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<Plan>> GetAllAsync()
    {
        return await _context.Plans
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }

    public async Task<Plan?> GetByIdAsync(Guid id)
    {
        return await _context.Plans
            .Where(p => !p.IsDeleted)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Plan plan)
    {
        await _context.Plans.AddAsync(plan);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Plan plan)
    {
        _context.Plans.Update(plan);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var p = await GetByIdAsync(id);
        if (p != null)
        {
            p.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
    
    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Plans.AnyAsync(p => p.Id == id && !p.IsDeleted);
    }
}
