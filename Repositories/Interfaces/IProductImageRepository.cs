using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Repositories.Interfaces;

public interface IProductImageRepository
{
    Task<IEnumerable<ProductImage>> GetByProductIdAsync(Guid productId);

    Task<ProductImage?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(ProductImage image);

    Task<bool> DeleteAsync(Guid id);

    Task<bool> SetPrimaryAsync(Guid productId, Guid imageId);
}