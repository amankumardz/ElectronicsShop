using ElectronicsShop.Data;
using ElectronicsShop.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsShop.Repositories;

public class RepairRepository : IRepairRepository
{
    private readonly ApplicationDbContext _db;

    public RepairRepository(ApplicationDbContext db) => _db = db;

    public Task<List<RepairRequest>> GetAllAsync() => _db.RepairRequests.Include(r => r.Customer).Include(r => r.StatusHistory).OrderByDescending(r => r.CreatedAtUtc).ToListAsync();

    public Task<List<RepairRequest>> GetByCustomerIdAsync(string customerId) =>
        _db.RepairRequests.Include(r => r.StatusHistory).Where(r => r.CustomerId == customerId).OrderByDescending(r => r.CreatedAtUtc).ToListAsync();

    public Task<RepairRequest?> GetByIdAsync(int id) => _db.RepairRequests.Include(r => r.StatusHistory).Include(r => r.Customer).FirstOrDefaultAsync(r => r.Id == id);

    public async Task AddAsync(RepairRequest request)
    {
        _db.RepairRequests.Add(request);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(RepairRequest request)
    {
        _db.RepairRequests.Update(request);
        await _db.SaveChangesAsync();
    }
}
