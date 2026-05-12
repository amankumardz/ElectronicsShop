using System.ComponentModel.DataAnnotations;
using ElectronicsShop.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace ElectronicsShop.ViewModels;

public class ManualStockEntryVm
{
    [Required] public int ProductId { get; set; }
    [Required] public string SupplierName { get; set; } = string.Empty;
    public string? SupplierGstin { get; set; }
    [Required] public string InvoiceNumber { get; set; } = string.Empty;
    [Required] public DateTime InvoiceDate { get; set; } = DateTime.Today;
    public string? HsnSac { get; set; }
    [Range(0.01, 999999)] public decimal Quantity { get; set; }
    [Required] public string UnitType { get; set; } = "PCS";
    [Range(0, 99999999)] public decimal PurchaseRate { get; set; }
    [Range(0, 100)] public decimal GstPercentage { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? ItemName { get; set; }
}

public class InvoiceParseResultVm
{
    public string? InvoiceNumber { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierGstin { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal Confidence { get; set; }
    public bool RequiresManualReview => Confidence < 0.65m;
    public string RawText { get; set; } = string.Empty;
    public string ExtractedJson { get; set; } = "{}";
    public List<ManualStockEntryVm> Items { get; set; } = new();
}

public class InvoiceUploadVm
{
    [Required] public IFormFile InvoiceFile { get; set; } = default!;
}

public class StockAdjustmentVm
{
    [Required] public int ProductId { get; set; }
    [Required] public StockChangeType ChangeType { get; set; }
    [Range(0,999999)] public decimal Quantity { get; set; }
    public string? Reason { get; set; }
}
