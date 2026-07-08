using System;
using Ostawy.Models;
using Ostawy.Repositories;
using Ostawy.ViewModels;

namespace Ostawy.Services;

public class ProfessionService
{
    private readonly ProfessionRepository _repository;

    public ProfessionService(ProfessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProfessionViewModel>> GetAllAsync()
    {
        var professions = await _repository.GetAllAsync();

        return professions.Select(x => new ProfessionViewModel
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();
    }

    public async Task<ProfessionViewModel?> GetByIdAsync(Guid id)
    {
        var profession = await _repository.GetByIdAsync(id);

        if (profession == null)
            return null;

        return new ProfessionViewModel
        {
            Id = profession.Id,
            Name = profession.Name
        };
    }

    public async Task CreateAsync(ProfessionViewModel vm)
    {
        var profession = new Profession
        {
            Id = Guid.NewGuid(),
            Name = vm.Name
        };

        await _repository.AddAsync(profession);
    }

    public async Task UpdateAsync(ProfessionViewModel vm)
    {
        var profession = await _repository.GetByIdAsync(vm.Id);

        if (profession == null)
            return;

        profession.Name = vm.Name;

        await _repository.UpdateAsync(profession);
    }

    public async Task DeleteAsync(Guid id)
    {
        var profession = await _repository.GetByIdAsync(id);

        if (profession == null)
            return;

        await _repository.DeleteAsync(profession);
    }
}
