namespace MrPorkStore.Api.Models;

public class ProductImage
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    // Cloudinary identifier, needed when deleting the actual image
    public string? PublicId { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }
}