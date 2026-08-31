using MrPorkStore.Api.DTOs.ProductImages;


namespace MrPorkStore.Api.Services.Interfaces;

public interface IProductImageService
{
    Task<ProductImageUploadResult> UploadAsync(
        Guid productId,
        IFormFile file);

    Task<bool> DeleteAsync(Guid imageId);
}