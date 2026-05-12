using ElectronicsShop.ViewModels;

namespace ElectronicsShop.Services;

public interface IInvoiceParserService
{
    Task<List<ManualStockEntryVm>> ParseAsync(string filePath);
}
