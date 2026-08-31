namespace MrPorkStore.Api.DTOs.ProductImages;

public class ProductImageUploadResult
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? PublicId { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }
}