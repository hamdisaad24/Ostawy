using Ostawy.Models;
using Ostawy.Repositories;
using Ostawy.ViewModels;

namespace Ostawy.Services;

public class PlanService
{
    private readonly PlanRepository _planRepo;

    public PlanService(PlanRepository planRepo)
    {
        _planRepo = planRepo;
    }

    public async Task<List<PlanViewModel>> GetAllAsync()
    {
        var plans = await _planRepo.GetAllAsync();

        return plans.Select(p => new PlanViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            MaxRequests = p.MaxRequests,
            AllowVideos = p.AllowVideos,
            HasPrioritySearch = p.HasPrioritySearch,
            HasVerifiedBadge = p.HasVerifiedBadge
        }).ToList();
    }

    public async Task<PlanViewModel?> GetByIdAsync(Guid id)
    {
        var plan = await _planRepo.GetByIdAsync(id);

        if (plan == null)
            return null;

        return new PlanViewModel
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            MaxRequests = plan.MaxRequests,
            AllowVideos = plan.AllowVideos,
            HasPrioritySearch = plan.HasPrioritySearch,
            HasVerifiedBadge = plan.HasVerifiedBadge
        };
    }

    public async Task CreateAsync(PlanViewModel vm)
    {
        var plan = new Plan
        {
            Id = Guid.NewGuid(),
            Name = vm.Name,
            Description = vm.Description,
            Price = vm.Price,
            MaxRequests = vm.MaxRequests,
            AllowVideos = vm.AllowVideos,
            HasPrioritySearch = vm.HasPrioritySearch,
            HasVerifiedBadge = vm.HasVerifiedBadge
        };

        await _planRepo.AddAsync(plan);
    }

    public async Task<bool> UpdateAsync(PlanViewModel vm)
    {
        var plan = await _planRepo.GetByIdAsync(vm.Id);

        if (plan == null)
            return false;

        plan.Name = vm.Name;
        plan.Description = vm.Description;
        plan.Price = vm.Price;
        plan.MaxRequests = vm.MaxRequests;
        plan.AllowVideos = vm.AllowVideos;
        plan.HasPrioritySearch = vm.HasPrioritySearch;
        plan.HasVerifiedBadge = vm.HasVerifiedBadge;

        await _planRepo.UpdateAsync(plan);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _planRepo.DeleteAsync(id);
    }
}