using ElectronicsShop.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ElectronicsShop.Models;

public class RepairRequest
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string DeviceName { get; set; } = string.Empty;

    [Required, StringLength(1000)]
    public string IssueDescription { get; set; } = string.Empty;

    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Phone]
    public string? CustomerPhone { get; set; }

    public RepairStatus Status { get; set; } = RepairStatus.Pending;

    [Range(0, 999999)]
    public decimal? EstimatedCost { get; set; }

    [StringLength(1000)]
    public string? AdminNotes { get; set; }

    public bool IsWalkIn { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }

    public string? CustomerId { get; set; }
    public ApplicationUser? Customer { get; set; }

    public ICollection<RepairStatusHistory> StatusHistory { get; set; } = new List<RepairStatusHistory>();
}
