using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Repositories.Interfaces;

public interface IDataNetworkRepository
{
    Task<IEnumerable<DataNetwork>> GetAllAsync();

    Task<IEnumerable<DataNetwork>> GetActiveAsync();

    Task<DataNetwork?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(DataNetwork network);

    Task<bool> UpdateAsync(DataNetwork network);

    Task<bool> DeleteAsync(Guid id);
}