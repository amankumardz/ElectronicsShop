using System.ComponentModel.DataAnnotations;

namespace ElectronicsShop.Models;

public class Sale
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Range(1, 10000)]
    public int Quantity { get; set; }

    [Range(0, 999999)]
    public decimal UnitPrice { get; set; }

    public DateTime SoldAtUtc { get; set; } = DateTime.UtcNow;
}
