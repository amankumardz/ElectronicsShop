using ElectronicsShop.Data;
using ElectronicsShop.Models;
using ElectronicsShop.Models.Enums;
using ElectronicsShop.Services;
using ElectronicsShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsShop.Controllers;

[Authorize(Roles = "Admin")]
public class StockController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly IInvoiceParserService _invoiceParser;
    private readonly UserManager<ApplicationUser> _userManager;

    public StockController(ApplicationDbContext db, IWebHostEnvironment env, IInvoiceParserService invoiceParser, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _env = env;
        _invoiceParser = invoiceParser;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index() => View(await _db.Products.Include(p => p.Category).OrderBy(p => p.Name).ToListAsync());

    public async Task<IActionResult> Add() { await LoadProducts(); return View(new ManualStockEntryVm()); }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(ManualStockEntryVm vm)
    {
        if (!ModelState.IsValid) { await LoadProducts(); return View(vm); }
        var product = await _db.Products.FindAsync(vm.ProductId); if (product == null) return NotFound();

        var invoice = new StockInvoice { SupplierName = vm.SupplierName, SupplierGstin = vm.SupplierGstin, InvoiceNumber = vm.InvoiceNumber, InvoiceDate = vm.InvoiceDate, Notes = vm.Notes };
        _db.StockInvoices.Add(invoice);

        var prev = product.StockQuantity;
        product.StockQuantity += vm.Quantity; ApplyStatus(product);

        _db.StockEntries.Add(new StockEntry { ProductId = product.Id, StockInvoice = invoice, ItemName = product.Name, HsnSac = vm.HsnSac, Quantity = vm.Quantity, UnitType = vm.UnitType, PurchaseRate = vm.PurchaseRate, GstPercentage = vm.GstPercentage, CgstAmount = vm.CgstAmount, SgstAmount = vm.SgstAmount, TaxableAmount = vm.TaxableAmount, TotalAmount = vm.TotalAmount, Notes = vm.Notes });
        _db.StockHistoryLogs.Add(new StockHistoryLog { ProductId = product.Id, ChangeType = StockChangeType.Add, QuantityChanged = vm.Quantity, PreviousStock = prev, NewStock = product.StockQuantity, InvoiceReference = vm.InvoiceNumber, ReasonNote = vm.Notes, AdminUserId = _userManager.GetUserId(User) });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult UploadInvoice() => View(new InvoiceUploadVm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadInvoice(InvoiceUploadVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var folder = Path.Combine(_env.WebRootPath, "uploads", "invoices"); Directory.CreateDirectory(folder);
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(vm.InvoiceFile.FileName)}";
        var fullPath = Path.Combine(folder, fileName);
        await using (var fs = System.IO.File.Create(fullPath)) await vm.InvoiceFile.CopyToAsync(fs);

        var items = await _invoiceParser.ParseAsync(fullPath);
        ViewBag.UploadedInvoicePath = $"/uploads/invoices/{fileName}";
        await LoadProducts();
        return View("InvoicePreview", items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Adjust(StockAdjustmentVm vm)
    {
        var product = await _db.Products.FindAsync(vm.ProductId); if (product == null) return NotFound();
        var prev = product.StockQuantity;
        if (vm.ChangeType == StockChangeType.OutOfStock) product.StockQuantity = 0;
        else product.StockQuantity = Math.Max(0, product.StockQuantity - vm.Quantity);
        ApplyStatus(product);

        _db.StockHistoryLogs.Add(new StockHistoryLog { ProductId = vm.ProductId, ChangeType = vm.ChangeType, QuantityChanged = vm.ChangeType == StockChangeType.OutOfStock ? prev : vm.Quantity, PreviousStock = prev, NewStock = product.StockQuantity, ReasonNote = vm.Reason, AdminUserId = _userManager.GetUserId(User) });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> History() => View(await _db.StockHistoryLogs.Include(x=>x.Product).Include(x=>x.AdminUser).OrderByDescending(x=>x.CreatedAt).Take(300).ToListAsync());

    private static void ApplyStatus(Product p) => p.StockStatus = p.StockQuantity <= 0 ? StockAvailabilityStatus.OutOfStock : p.StockQuantity <= p.MinimumStockLevel ? StockAvailabilityStatus.LowStock : StockAvailabilityStatus.InStock;
    private async Task LoadProducts() => ViewBag.ProductId = new SelectList(await _db.Products.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
}
