using ElectronicsShop.ViewModels;

namespace ElectronicsShop.Services;

public class InvoiceParserService : IInvoiceParserService
{
    public Task<List<ManualStockEntryVm>> ParseAsync(string filePath)
    {
        // Placeholder OCR/parser hook.
        // Integrate Azure Document Intelligence, Tesseract, or custom invoice ML extraction here.
        // Return normalized line items so admin can review/edit before saving.
        return Task.FromResult(new List<ManualStockEntryVm>());
    }
}
