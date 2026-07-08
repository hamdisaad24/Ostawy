using System;
using Ostawy.Data;
using Ostawy.Models;
using Microsoft.EntityFrameworkCore;

namespace Ostawy.Repositories;

public class ProfessionRepository
{
    private readonly ApplicationDbContext _context;

    public ProfessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Profession>> GetAllAsync()
    {
        return await _context.Professions
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Profession?> GetByIdAsync(Guid id)
    {
        return await _context.Professions.FindAsync(id);
    }

    public async Task AddAsync(Profession profession)
    {
        _context.Professions.Add(profession);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Profession profession)
    {
        _context.Professions.Update(profession);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Profession profession)
    {
        _context.Professions.Remove(profession);
        await _context.SaveChangesAsync();
    }
}
