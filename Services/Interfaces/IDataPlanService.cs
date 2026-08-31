using MrPorkStore.Api.DTOs.DataPlans;

namespace MrPorkStore.Api.Services.Interfaces;

public interface IDataPlanService
{
    Task<IEnumerable<DataPlanResponse>> GetAllAsync();

    Task<IEnumerable<DataPlanResponse>> GetAvailableAsync();

    Task<IEnumerable<DataPlanResponse>> GetByNetworkIdAsync(
        Guid networkId);

    Task<DataPlanResponse?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(CreateDataPlanRequest request);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateDataPlanRequest request);

    Task<bool> DeleteAsync(Guid id);
}