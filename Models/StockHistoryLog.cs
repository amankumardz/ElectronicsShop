using System.ComponentModel.DataAnnotations;
using ElectronicsShop.Models.Enums;

namespace ElectronicsShop.Models;

public class StockHistoryLog
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public StockChangeType ChangeType { get; set; }
    public decimal QuantityChanged { get; set; }
    public decimal PreviousStock { get; set; }
    public decimal NewStock { get; set; }

    [StringLength(100)]
    public string? InvoiceReference { get; set; }

    [StringLength(1000)]
    public string? ReasonNote { get; set; }

    public string? AdminUserId { get; set; }
    public ApplicationUser? AdminUser { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
