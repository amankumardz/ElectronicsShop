using ElectronicsShop.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ElectronicsShop.Models;

public class RepairStatusHistory
{
    public int Id { get; set; }

    public int RepairRequestId { get; set; }
    public RepairRequest? RepairRequest { get; set; }

    public RepairStatus Status { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }

    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;

    public string? ChangedByUserId { get; set; }
}
