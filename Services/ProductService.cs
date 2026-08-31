using MrPorkStore.Api.DTOs.Products;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(MapToResponse);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        return product is null
            ? null
            : MapToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),

            Category = request.Category.Trim(),
            ProductType = request.ProductType.Trim(),

            WeightOrSize = string.IsNullOrWhiteSpace(request.WeightOrSize)
                ? null
                : request.WeightOrSize.Trim(),

            Price = request.Price,
            StockQuantity = request.StockQuantity,

            IsAvailable = request.IsAvailable,
            IsActive = true
        };

        var id = await _productRepository.CreateAsync(product);

        var createdProduct =
            await _productRepository.GetByIdAsync(id);

        if (createdProduct is null)
        {
            throw new InvalidOperationException(
                "Product was created but could not be retrieved.");
        }

        return MapToResponse(createdProduct);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateProductRequest request)
    {
        var existingProduct =
            await _productRepository.GetByIdAsync(id);

        if (existingProduct is null)
            return false;

        existingProduct.Name = request.Name.Trim();

        existingProduct.Description =
            string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();

        existingProduct.Category = request.Category.Trim();

        existingProduct.ProductType =
            request.ProductType.Trim();

        existingProduct.WeightOrSize =
            string.IsNullOrWhiteSpace(request.WeightOrSize)
                ? null
                : request.WeightOrSize.Trim();

        existingProduct.Price = request.Price;

        existingProduct.StockQuantity =
            request.StockQuantity;

        existingProduct.IsAvailable =
            request.IsAvailable;

        existingProduct.IsActive =
            request.IsActive;

        return await _productRepository.UpdateAsync(
            existingProduct);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _productRepository.DeleteAsync(id);
    }

    private static ProductResponse MapToResponse(
        Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Category = product.Category,
            ProductType = product.ProductType,
            WeightOrSize = product.WeightOrSize,

            Price = product.Price ?? 0,

            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }
}