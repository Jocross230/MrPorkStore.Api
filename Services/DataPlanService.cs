using MrPorkStore.Api.DTOs.DataPlans;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Services;

public class DataPlanService : IDataPlanService
{
    private readonly IDataPlanRepository _repository;
    private readonly IDataNetworkRepository _networkRepository;

    public DataPlanService(
        IDataPlanRepository repository,
        IDataNetworkRepository networkRepository)
    {
        _repository = repository;
        _networkRepository = networkRepository;
    }

    public async Task<IEnumerable<DataPlanResponse>> GetAllAsync()
    {
        var plans = await _repository.GetAllAsync();

        return await MapToResponsesAsync(plans);
    }

    public async Task<IEnumerable<DataPlanResponse>> GetAvailableAsync()
    {
        var plans = await _repository.GetAvailableAsync();

        var availablePlans = new List<DataPlanResponse>();

        foreach (var plan in plans)
        {
            var network = await _networkRepository.GetByIdAsync(
                plan.NetworkId);

            if (network is null || !network.IsActive)
            {
                continue;
            }

            availablePlans.Add(MapToResponse(plan, network));
        }

        return availablePlans;
    }

    public async Task<IEnumerable<DataPlanResponse>> GetByNetworkIdAsync(
        Guid networkId)
    {
        var network = await _networkRepository.GetByIdAsync(
            networkId);

        if (network is null || !network.IsActive)
        {
            return Enumerable.Empty<DataPlanResponse>();
        }

        var plans = await _repository.GetByNetworkIdAsync(networkId);

        return plans.Select(plan =>
            MapToResponse(plan, network));
    }

    public async Task<DataPlanResponse?> GetByIdAsync(Guid id)
    {
        var plan = await _repository.GetByIdAsync(id);

        if (plan is null)
        {
            return null;
        }

        var network = await _networkRepository.GetByIdAsync(
            plan.NetworkId);

        if (network is null)
        {
            return null;
        }

        return MapToResponse(plan, network);
    }

    public async Task<Guid> CreateAsync(
        CreateDataPlanRequest request)
    {
        ValidateRequest(
            request.Name,
            request.DataSize,
            request.Price);

        var network = await _networkRepository.GetByIdAsync(
            request.NetworkId);

        if (network is null)
        {
            throw new KeyNotFoundException(
                "Data network not found.");
        }

        if (!network.IsActive)
        {
            throw new InvalidOperationException(
                "Cannot create a plan for an inactive network.");
        }

        var plan = new DataPlan
        {
            NetworkId = request.NetworkId,
            Name = request.Name.Trim(),
            DataSize = request.DataSize.Trim(),
            Validity = string.IsNullOrWhiteSpace(request.Validity)
                ? null
                : request.Validity.Trim(),
            Price = request.Price,
            IsAvailable = true
        };

        return await _repository.CreateAsync(plan);
    }

    public async Task<bool> UpdateAsync(
        Guid id,
        UpdateDataPlanRequest request)
    {
        ValidateRequest(
            request.Name,
            request.DataSize,
            request.Price);

        var existing = await _repository.GetByIdAsync(id);

        if (existing is null)
        {
            return false;
        }

        var network = await _networkRepository.GetByIdAsync(
            request.NetworkId);

        if (network is null)
        {
            throw new KeyNotFoundException(
                "Data network not found.");
        }

        existing.NetworkId = request.NetworkId;
        existing.Name = request.Name.Trim();
        existing.DataSize = request.DataSize.Trim();
        existing.Validity = string.IsNullOrWhiteSpace(request.Validity)
            ? null
            : request.Validity.Trim();
        existing.Price = request.Price;
        existing.IsAvailable = request.IsAvailable;

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

    private async Task<IEnumerable<DataPlanResponse>> MapToResponsesAsync(
        IEnumerable<DataPlan> plans)
    {
        var responses = new List<DataPlanResponse>();

        foreach (var plan in plans)
        {
            var network = await _networkRepository.GetByIdAsync(
                plan.NetworkId);

            if (network is null)
            {
                continue;
            }

            responses.Add(MapToResponse(plan, network));
        }

        return responses;
    }

    private static DataPlanResponse MapToResponse(
        DataPlan plan,
        DataNetwork network)
    {
        return new DataPlanResponse
        {
            Id = plan.Id,
            NetworkId = plan.NetworkId,
            NetworkName = network.Name,
            NetworkLogoUrl = network.LogoUrl,
            Name = plan.Name,
            DataSize = plan.DataSize,
            Validity = plan.Validity,
            Price = plan.Price,
            IsAvailable = plan.IsAvailable,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }

    private static void ValidateRequest(
        string name,
        string dataSize,
        decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Data plan name is required.");
        }

        if (string.IsNullOrWhiteSpace(dataSize))
        {
            throw new ArgumentException(
                "Data size is required.");
        }

        if (price <= 0)
        {
            throw new ArgumentException(
                "Price must be greater than zero.");
        }
    }
}