using System.ComponentModel.DataAnnotations;

namespace ElectronicsShop.Models;

public class Product
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, 999999)]
    public decimal Price { get; set; }

    [StringLength(50)]
    public string? Unit { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
