using MrPorkStore.Api.DTOs.ProductVariants;

namespace MrPorkStore.Api.Services.Interfaces;

public interface IProductVariantService
{
    Task<IEnumerable<ProductVariantResponse>> GetByProductIdAsync(Guid productId);

    Task<ProductVariantResponse?> CreateAsync(
        Guid productId,
        CreateProductVariantRequest request);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateProductVariantRequest request);

    Task<bool> DeleteAsync(Guid id);
}