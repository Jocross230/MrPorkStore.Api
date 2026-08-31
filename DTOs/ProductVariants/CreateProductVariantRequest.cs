using System.ComponentModel.DataAnnotations;

namespace MrPorkStore.Api.DTOs.ProductVariants;

public class CreateProductVariantRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? WeightOrSize { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int? StockQuantity { get; set; }

    public bool IsAvailable { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; } = 0;
}