using MrPorkStore.Api.DTOs.AppSettings;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Services;

public class AppSettingService : IAppSettingService
{
    private readonly IAppSettingRepository _repository;

    public AppSettingService(
        IAppSettingRepository repository)
    {
        _repository = repository;
    }

    public async Task<AppSettingResponse?> GetAsync()
    {
        var setting = await _repository.GetAsync();

        if (setting is null)
        {
            return null;
        }

        return MapToResponse(setting);
    }

    public async Task<AppSettingResponse> UpdateAsync(
        UpdateAppSettingRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.WhatsappNumber))
        {
            throw new ArgumentException(
                "WhatsApp number is required.");
        }

        if (string.IsNullOrWhiteSpace(request.BusinessName))
        {
            throw new ArgumentException(
                "Business name is required.");
        }

        var setting = await _repository.GetAsync();

        if (setting is null)
        {
            throw new InvalidOperationException(
                "Application settings have not been configured.");
        }

        setting.WhatsappNumber = request.WhatsappNumber.Trim();
        setting.BusinessName = request.BusinessName.Trim();

        var updated = await _repository.UpdateAsync(setting);

        if (!updated)
        {
            throw new InvalidOperationException(
                "Application settings could not be updated.");
        }

        var updatedSetting = await _repository.GetAsync();

        if (updatedSetting is null)
        {
            throw new InvalidOperationException(
                "Application settings could not be retrieved after update.");
        }

        return MapToResponse(updatedSetting);
    }

    private static AppSettingResponse MapToResponse(
        AppSetting setting)
    {
        return new AppSettingResponse
        {
            Id = setting.Id,
            WhatsappNumber = setting.WhatsappNumber,
            BusinessName = setting.BusinessName,
            CreatedAt = setting.CreatedAt,
            UpdatedAt = setting.UpdatedAt
        };
    }
}