using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Repositories.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(Product product);

    Task<bool> UpdateAsync(Product product);

    Task<bool> DeleteAsync(Guid id);
}