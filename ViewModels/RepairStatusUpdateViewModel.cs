using ElectronicsShop.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ElectronicsShop.ViewModels;

public class RepairStatusUpdateViewModel
{
    public int Id { get; set; }

    [Required]
    public RepairStatus Status { get; set; }

    [Range(0, 999999)]
    public decimal? EstimatedCost { get; set; }

    [StringLength(1000)]
    public string? AdminNotes { get; set; }
}
