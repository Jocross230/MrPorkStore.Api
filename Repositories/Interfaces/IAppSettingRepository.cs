using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Repositories.Interfaces;

public interface IAppSettingRepository
{
    Task<AppSetting?> GetAsync();

    Task<bool> UpdateAsync(AppSetting setting);
}