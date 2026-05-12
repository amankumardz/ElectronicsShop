using System.ComponentModel.DataAnnotations;

namespace ElectronicsShop.Models;

public class StockInvoice
{
    public int Id { get; set; }

    [StringLength(200)]
    public string? SupplierName { get; set; }

    [StringLength(50)]
    public string? SupplierGstin { get; set; }

    [StringLength(100)]
    public string? InvoiceNumber { get; set; }

    public DateTime? InvoiceDate { get; set; }

    [StringLength(500)]
    public string? FilePath { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    public ICollection<StockEntry> StockEntries { get; set; } = new List<StockEntry>();
}
