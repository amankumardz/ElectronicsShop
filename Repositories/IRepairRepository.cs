using ElectronicsShop.Models;

namespace ElectronicsShop.Repositories;

public interface IRepairRepository
{
    Task<List<RepairRequest>> GetAllAsync();
    Task<List<RepairRequest>> GetByCustomerIdAsync(string customerId);
    Task<RepairRequest?> GetByIdAsync(int id);
    Task AddAsync(RepairRequest request);
    Task UpdateAsync(RepairRequest request);
}
