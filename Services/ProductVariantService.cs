using MrPorkStore.Api.DTOs.ProductVariants;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Services;

public class ProductVariantService : IProductVariantService
{
    private readonly IProductVariantRepository _variantRepository;
    private readonly IProductRepository _productRepository;

    public ProductVariantService(
        IProductVariantRepository variantRepository,
        IProductRepository productRepository)
    {
        _variantRepository = variantRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductVariantResponse>> GetByProductIdAsync(
        Guid productId)
    {
        var variants =
            await _variantRepository.GetByProductIdAsync(productId);

        return variants.Select(MapToResponse);
    }

    public async Task<ProductVariantResponse?> CreateAsync(
        Guid productId,
        CreateProductVariantRequest request)
    {
        // Important: do not allow variants for products
        // that do not exist.
        var product =
            await _productRepository.GetByIdAsync(productId);

        if (product is null)
            return null;

        var variant = new ProductVariant
        {
            ProductId = productId,
            Name = request.Name.Trim(),

            WeightOrSize =
                string.IsNullOrWhiteSpace(request.WeightOrSize)
                    ? null
                    : request.WeightOrSize.Trim(),

            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsAvailable = request.IsAvailable,
            DisplayOrder = request.DisplayOrder
        };

        var id = await _variantRepository.CreateAsync(variant);

        var createdVariant =
            await _variantRepository.GetByIdAsync(id);

        if (createdVariant is null)
        {
            throw new InvalidOperationException(
                "Product variant was created but could not be retrieved.");
        }

        return MapToResponse(createdVariant);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateProductVariantRequest request)
    {
        var variant =
            await _variantRepository.GetByIdAsync(id);

        if (variant is null)
            return false;

        variant.Name = request.Name.Trim();

        variant.WeightOrSize =
            string.IsNullOrWhiteSpace(request.WeightOrSize)
                ? null
                : request.WeightOrSize.Trim();

        variant.Price = request.Price;
        variant.StockQuantity = request.StockQuantity;
        variant.IsAvailable = request.IsAvailable;
        variant.DisplayOrder = request.DisplayOrder;

        return await _variantRepository.UpdateAsync(variant);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var variant =
            await _variantRepository.GetByIdAsync(id);

        if (variant is null)
            return false;

        return await _variantRepository.DeleteAsync(id);
    }

    private static ProductVariantResponse MapToResponse(
        ProductVariant variant)
    {
        return new ProductVariantResponse
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            Name = variant.Name,
            WeightOrSize = variant.WeightOrSize,
            Price = variant.Price,
            StockQuantity = variant.StockQuantity,
            IsAvailable = variant.IsAvailable,
            DisplayOrder = variant.DisplayOrder
        };
    }
}