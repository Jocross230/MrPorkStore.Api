using MrPorkStore.Api.DTOs.DataNetworks;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Services;

public class DataNetworkService : IDataNetworkService
{
    private readonly IDataNetworkRepository _repository;

    public DataNetworkService(IDataNetworkRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DataNetworkResponse>> GetAllAsync()
    {
        var networks = await _repository.GetAllAsync();

        return networks.Select(MapToResponse);
    }

    public async Task<IEnumerable<DataNetworkResponse>> GetActiveAsync()
    {
        var networks = await _repository.GetActiveAsync();

        return networks.Select(MapToResponse);
    }

    public async Task<DataNetworkResponse?> GetByIdAsync(Guid id)
    {
        var network = await _repository.GetByIdAsync(id);

        return network is null
            ? null
            : MapToResponse(network);
    }

    public async Task<Guid> CreateAsync(
        CreateDataNetworkRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Network name is required.");
        }

        var network = new DataNetwork
        {
            Name = request.Name.Trim(),
            LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl)
                ? null
                : request.LogoUrl.Trim(),
            IsActive = true
        };

        return await _repository.CreateAsync(network);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateDataNetworkRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException(
                "Network name is required.");
        }

        var existing = await _repository.GetByIdAsync(id);

        if (existing is null)
        {
            return false;
        }

        existing.Name = request.Name.Trim();
        existing.LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl)
            ? null
            : request.LogoUrl.Trim();
        existing.IsActive = request.IsActive;

        return await _repository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);

        if (existing is null)
        {
            return false;
        }

        return await _repository.DeleteAsync(id);
    }

    private static DataNetworkResponse MapToResponse(
        DataNetwork network)
    {
        return new DataNetworkResponse
        {
            Id = network.Id,
            Name = network.Name,
            LogoUrl = network.LogoUrl,
            IsActive = network.IsActive,
            CreatedAt = network.CreatedAt,
            UpdatedAt = network.UpdatedAt
        };
    }
}