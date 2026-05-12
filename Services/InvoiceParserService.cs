using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using ElectronicsShop.ViewModels;
using Tesseract;

namespace ElectronicsShop.Services;

public partial class InvoiceParserService(ILogger<InvoiceParserService> logger, IWebHostEnvironment env) : IInvoiceParserService
{
    public Task<InvoiceParseResultVm> ParseAsync(string filePath)
    {
        try
        {
            var rawText = ReadText(filePath);
            var result = ParseText(rawText);
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "OCR extraction failed for {FilePath}", filePath);
            return Task.FromResult(new InvoiceParseResultVm());
        }
    }

    private string ReadText(string filePath)
    {
        var ext = Path.GetExtension(filePath).ToLowerInvariant();

        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
            return ReadImageWithTesseract(filePath);

        if (ext == ".pdf")
            throw new NotSupportedException("PDF invoice parsing is not supported yet. Please upload JPG, JPEG, or PNG invoice images.");

        throw new NotSupportedException($"Unsupported invoice format: {ext}");
    }

    private string ReadImageWithTesseract(string filePath)
    {
        var tessData = Path.Combine(env.ContentRootPath, "tessdata");
        if (!Directory.Exists(tessData))
        {
            logger.LogWarning("tessdata folder missing at {Path}. OCR quality may be poor or fail.", tessData);
        }

        using var engine = new TesseractEngine(tessData, "eng", EngineMode.Default);
        using var img = Pix.LoadFromFile(filePath);
        using var page = engine.Process(img);
        return page.GetText() ?? string.Empty;
    }

    private InvoiceParseResultVm ParseText(string text)
    {
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new InvoiceParseResultVm
        {
            RawText = text,
            InvoiceNumber = FirstGroup(text, @"(?:invoice\s*(?:no|number)?\s*[:#-]?\s*)([A-Z0-9\/-]+)"),
            SupplierGstin = FirstGroup(text, @"\b([0-9]{2}[A-Z]{5}[0-9]{4}[A-Z][A-Z0-9]Z[A-Z0-9])\b"),
            SupplierName = lines.FirstOrDefault(l => Regex.IsMatch(l, "(traders|electronics|enterprise|solutions|pvt|ltd)", RegexOptions.IgnoreCase)),
            InvoiceDate = ParseDate(FirstGroup(text, @"(?:invoice\s*date|date)\s*[:\-]?\s*([0-9]{1,2}[\/-][0-9]{1,2}[\/-][0-9]{2,4})")),
            TotalAmount = ParseDecimal(FirstGroup(text, @"(?:grand\s*total|total\s*amount|net\s*amount)\s*[:\-]?\s*₹?\s*([0-9,]+(?:\.[0-9]{1,2})?)"))
        };

        result.Items = ExtractRows(lines, result);
        result.Confidence = Score(result);
        result.ExtractedJson = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        return result;
    }

    private static List<ManualStockEntryVm> ExtractRows(string[] lines, InvoiceParseResultVm header)
    {
        var rows = new List<ManualStockEntryVm>();
        foreach (var line in lines)
        {
            if (!Regex.IsMatch(line, @"\d") || line.Length < 8) continue;

            var qty = ParseDecimal(FirstGroup(line, @"(\d+(?:\.\d+)?)\s*PCS"))
                      ?? ParseDecimal(FirstGroup(line, @"\bQty\s*[:\-]?\s*(\d+(?:\.\d+)?)"));
            var rate = ParseDecimal(FirstGroup(line, @"(?:rate|price)\s*[:\-]?\s*₹?\s*([0-9,]+(?:\.[0-9]{1,2})?)"))
                       ?? ParseDecimal(FirstGroup(line, @"\s₹\s*([0-9,]+(?:\.[0-9]{1,2})?)"));
            var gst = ParseDecimal(FirstGroup(line, @"(\d{1,2}(?:\.\d+)?)\s*%"));
            var total = ParseDecimal(FirstGroup(line, @"(?:amt|amount|total)\s*[:\-]?\s*₹?\s*([0-9,]+(?:\.[0-9]{1,2})?)"));

            if (qty is null && rate is null && total is null) continue;

            var name = Regex.Replace(line, @"\s+", " ").Trim();
            name = Regex.Replace(name, @"₹?[0-9,]+(?:\.[0-9]{1,2})?|\d+(?:\.\d+)?\s*PCS|\d{1,2}(?:\.\d+)?%", "", RegexOptions.IgnoreCase).Trim();

            rows.Add(new ManualStockEntryVm
            {
                ItemName = string.IsNullOrWhiteSpace(name) ? "Parsed item" : name,
                SupplierName = header.SupplierName ?? "Unknown Supplier",
                SupplierGstin = header.SupplierGstin,
                InvoiceNumber = header.InvoiceNumber ?? "Unknown",
                InvoiceDate = header.InvoiceDate ?? DateTime.Today,
                Quantity = qty ?? 0,
                PurchaseRate = rate ?? 0,
                GstPercentage = gst ?? 0,
                TotalAmount = total ?? ((qty ?? 0) * (rate ?? 0)),
                UnitType = "PCS"
            });
        }

        return rows;
    }

    private static decimal Score(InvoiceParseResultVm r)
    {
        var score = 0m;
        if (!string.IsNullOrWhiteSpace(r.InvoiceNumber)) score += 0.15m;
        if (r.InvoiceDate.HasValue) score += 0.1m;
        if (!string.IsNullOrWhiteSpace(r.SupplierName)) score += 0.15m;
        if (!string.IsNullOrWhiteSpace(r.SupplierGstin)) score += 0.2m;
        if (r.TotalAmount.HasValue) score += 0.1m;
        if (r.Items.Count > 0) score += 0.3m;
        return Math.Min(score, 1m);
    }

    private static string? FirstGroup(string text, string pattern)
    {
        var m = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
        return m.Success ? m.Groups[1].Value.Trim() : null;
    }

    private static DateTime? ParseDate(string? value)
    {
        if (value is null) return null;
        return DateTime.TryParse(value, out var d) ? d : null;
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        value = value.Replace(",", "").Replace("₹", "").Trim();
        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : null;
    }
}
