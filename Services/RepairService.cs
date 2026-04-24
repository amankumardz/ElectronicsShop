using ElectronicsShop.Models;
using ElectronicsShop.Models.Enums;
using ElectronicsShop.Repositories;

namespace ElectronicsShop.Services;

public class RepairService : IRepairService
{
    private readonly IRepairRepository _repairRepository;

    public RepairService(IRepairRepository repairRepository)
    {
        _repairRepository = repairRepository;
    }

    public Task<List<RepairRequest>> GetAllAsync() => _repairRepository.GetAllAsync();

    public Task<List<RepairRequest>> GetForCustomerAsync(string customerId) => _repairRepository.GetByCustomerIdAsync(customerId);

    public Task<RepairRequest?> GetByIdAsync(int id) => _repairRepository.GetByIdAsync(id);

    public async Task CreateAsync(RepairRequest request, string? changedByUserId)
    {
        request.StatusHistory.Add(new RepairStatusHistory
        {
            Status = request.Status,
            Note = "Repair request created",
            ChangedByUserId = changedByUserId
        });

        await _repairRepository.AddAsync(request);
    }

    public async Task UpdateStatusAsync(int id, RepairStatus status, string? notes, decimal? estimatedCost, string? changedByUserId)
    {
        var repair = await _repairRepository.GetByIdAsync(id);
        if (repair == null) return;

        repair.Status = status;
        repair.AdminNotes = notes;
        repair.EstimatedCost = estimatedCost;
        repair.UpdatedAtUtc = DateTime.UtcNow;
        repair.StatusHistory.Add(new RepairStatusHistory
        {
            Status = status,
            Note = notes,
            ChangedByUserId = changedByUserId
        });

        await _repairRepository.UpdateAsync(repair);
    }
}
