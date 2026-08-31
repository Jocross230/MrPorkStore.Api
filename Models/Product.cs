namespace MrPorkStore.Api.Models;

public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Category { get; set; } = string.Empty;

    public string ProductType { get; set; } = string.Empty;

    public string? WeightOrSize { get; set; }

    public decimal? Price { get; set; }

    public int? StockQuantity { get; set; }

    public bool IsAvailable { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}