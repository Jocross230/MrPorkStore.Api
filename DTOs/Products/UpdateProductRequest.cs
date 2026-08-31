using System.ComponentModel.DataAnnotations;

namespace MrPorkStore.Api.DTOs.Products;

public class UpdateProductRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string ProductType { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? WeightOrSize { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int? StockQuantity { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsActive { get; set; }
}