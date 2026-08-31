using MrPorkStore.Api.DTOs.Products;

namespace MrPorkStore.Api.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductResponse>> GetAllAsync();

    Task<ProductResponse?> GetByIdAsync(Guid id);

    Task<ProductResponse> CreateAsync(CreateProductRequest request);

    Task<bool> UpdateAsync(Guid id, UpdateProductRequest request);

    Task<bool> DeleteAsync(Guid id);
}