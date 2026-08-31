using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Repositories.Interfaces;

public interface IDataPlanRepository
{
    Task<IEnumerable<DataPlan>> GetAllAsync();

    Task<IEnumerable<DataPlan>> GetAvailableAsync();

    Task<IEnumerable<DataPlan>> GetByNetworkIdAsync(Guid networkId);

    Task<DataPlan?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(DataPlan plan);

    Task<bool> UpdateAsync(DataPlan plan);

    Task<bool> DeleteAsync(Guid id);
}