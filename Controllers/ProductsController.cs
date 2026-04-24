using ElectronicsShop.Data;
using ElectronicsShop.Models;
using ElectronicsShop.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsShop.Controllers;

public class ProductsController : Controller
{
    private readonly IProductRepository _productRepository;
    private readonly ApplicationDbContext _db;

    public ProductsController(IProductRepository productRepository, ApplicationDbContext db)
    {
        _productRepository = productRepository;
        _db = db;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var products = await _productRepository.GetAllAsync();
        return View(products);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? NotFound() : View(product);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminIndex()
    {
        var products = await _productRepository.GetAllAsync();
        return View(products);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync();
        return View(new Product());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(product);
        }

        await _productRepository.AddAsync(product);
        return RedirectToAction(nameof(AdminIndex));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();
        await LoadCategoriesAsync();
        return View(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Product product)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync();
            return View(product);
        }

        await _productRepository.UpdateAsync(product);
        return RedirectToAction(nameof(AdminIndex));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productRepository.DeleteAsync(id);
        return RedirectToAction(nameof(AdminIndex));
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _db.Categories.AsNoTracking().ToListAsync();
        ViewBag.CategoryId = new SelectList(categories, "Id", "Name");
    }
}
