using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ElectronicsShop.Models;

public class ApplicationUser : IdentityUser
{
    [StringLength(100)]
    public string? FullName { get; set; }

    public ICollection<RepairRequest> RepairRequests { get; set; } = new List<RepairRequest>();
}
