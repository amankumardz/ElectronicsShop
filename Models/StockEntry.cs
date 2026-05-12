using System.ComponentModel.DataAnnotations;

namespace ElectronicsShop.Models;

public class StockEntry
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int? StockInvoiceId { get; set; }
    public StockInvoice? StockInvoice { get; set; }

    [StringLength(150)]
    public string? ItemName { get; set; }

    [StringLength(40)]
    public string? HsnSac { get; set; }

    [Range(0, 999999)]
    public decimal Quantity { get; set; }

    [StringLength(20)]
    public string UnitType { get; set; } = "PCS";

    [Range(0, 99999999)]
    public decimal PurchaseRate { get; set; }

    [Range(0, 100)]
    public decimal GstPercentage { get; set; }

    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TotalAmount { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
