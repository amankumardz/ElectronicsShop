using ElectronicsShop.Models;
using ElectronicsShop.Models.Enums;

namespace ElectronicsShop.Services;

public interface IRepairService
{
    Task<List<RepairRequest>> GetAllAsync();
    Task<List<RepairRequest>> GetForCustomerAsync(string customerId);
    Task<RepairRequest?> GetByIdAsync(int id);
    Task CreateAsync(RepairRequest request, string? changedByUserId);
    Task UpdateStatusAsync(int id, RepairStatus status, string? notes, decimal? estimatedCost, string? changedByUserId);
}
