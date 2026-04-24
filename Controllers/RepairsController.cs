using ElectronicsShop.Models;
using ElectronicsShop.Models.Enums;
using ElectronicsShop.Services;
using ElectronicsShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicsShop.Controllers;

[Authorize]
public class RepairsController : Controller
{
    private readonly IRepairService _repairService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RepairsController(IRepairService repairService, UserManager<ApplicationUser> userManager)
    {
        _repairService = repairService;
        _userManager = userManager;
    }

    public async Task<IActionResult> MyRepairs()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null) return Challenge();

        var repairs = await _repairService.GetForCustomerAsync(userId);
        return View(repairs);
    }

    public IActionResult Create() => View(new RepairRequest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RepairRequest request)
    {
        if (!ModelState.IsValid) return View(request);

        request.CustomerId = _userManager.GetUserId(User);
        request.IsWalkIn = false;
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            var user = await _userManager.GetUserAsync(User);
            request.CustomerName = user?.FullName ?? User.Identity?.Name ?? "Customer";
        }

        await _repairService.CreateAsync(request, request.CustomerId);
        return RedirectToAction(nameof(MyRepairs));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var repairs = await _repairService.GetAllAsync();
        return View(repairs);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult WalkInCreate() => View("Create", new RepairRequest { IsWalkIn = true });

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> WalkInCreate(RepairRequest request)
    {
        if (!ModelState.IsValid) return View("Create", request);

        request.IsWalkIn = true;
        request.CustomerId = null;
        await _repairService.CreateAsync(request, _userManager.GetUserId(User));
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id)
    {
        var repair = await _repairService.GetByIdAsync(id);
        if (repair == null) return NotFound();

        return View(new RepairStatusUpdateViewModel
        {
            Id = repair.Id,
            Status = repair.Status,
            EstimatedCost = repair.EstimatedCost,
            AdminNotes = repair.AdminNotes
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(RepairStatusUpdateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await _repairService.UpdateStatusAsync(vm.Id, vm.Status, vm.AdminNotes, vm.EstimatedCost, _userManager.GetUserId(User));
        return RedirectToAction(nameof(Index));
    }
}
