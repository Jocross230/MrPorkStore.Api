using MrPorkStore.Api.DTOs.AppSettings;

namespace MrPorkStore.Api.Services.Interfaces;

public interface IAppSettingService
{
    Task<AppSettingResponse?> GetAsync();

    Task<AppSettingResponse> UpdateAsync(
        UpdateAppSettingRequest request);
}