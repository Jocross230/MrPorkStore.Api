using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Repositories.Interfaces;

public interface IProductVariantRepository
{
    Task<IEnumerable<ProductVariant>> GetByProductIdAsync(Guid productId);

    Task<ProductVariant?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(ProductVariant variant);

    Task<bool> UpdateAsync(ProductVariant variant);

    Task<bool> DeleteAsync(Guid id);
}