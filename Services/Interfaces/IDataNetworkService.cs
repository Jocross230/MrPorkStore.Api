using MrPorkStore.Api.DTOs.DataNetworks;

namespace MrPorkStore.Api.Services.Interfaces;

public interface IDataNetworkService
{
    Task<IEnumerable<DataNetworkResponse>> GetAllAsync();

    Task<IEnumerable<DataNetworkResponse>> GetActiveAsync();

    Task<DataNetworkResponse?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(CreateDataNetworkRequest request);

    Task<bool> UpdateAsync(
        Guid id,
        UpdateDataNetworkRequest request);

    Task<bool> DeleteAsync(Guid id);
}