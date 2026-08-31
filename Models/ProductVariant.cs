namespace MrPorkStore.Api.Models;

public class ProductVariant
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? WeightOrSize { get; set; }

    public decimal Price { get; set; }

    public int? StockQuantity { get; set; }

    public bool IsAvailable { get; set; }

    public int DisplayOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}