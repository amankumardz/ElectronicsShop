using ElectronicsShop.ViewModels;

namespace ElectronicsShop.Services;

public interface IInvoiceParserService
{
    Task<InvoiceParseResultVm> ParseAsync(string filePath);
}
