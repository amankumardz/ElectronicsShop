using ElectronicsShop.Data;
using ElectronicsShop.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsShop.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db) => _db = db;

    public Task<List<Product>> GetAllAsync() => _db.Products.Include(p => p.Category).AsNoTracking().ToListAsync();

    public Task<Product?> GetByIdAsync(int id) => _db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _db.Products.Update(product);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) return;
        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
    }
}
