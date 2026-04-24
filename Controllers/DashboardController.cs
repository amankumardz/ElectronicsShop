using ElectronicsShop.Data;
using ElectronicsShop.Models.Enums;
using ElectronicsShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsShop.Controllers;

[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;

    public DashboardController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new DashboardViewModel
        {
            TotalProducts = await _db.Products.CountAsync(),
            TotalRepairs = await _db.RepairRequests.CountAsync(),
            PendingRepairs = await _db.RepairRequests.CountAsync(r => r.Status == RepairStatus.Pending),
            CompletedRepairs = await _db.RepairRequests.CountAsync(r => r.Status == RepairStatus.Completed || r.Status == RepairStatus.Delivered)
        };

        return View(vm);
    }
}
