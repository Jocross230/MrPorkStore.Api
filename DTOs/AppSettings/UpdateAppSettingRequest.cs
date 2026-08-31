namespace MrPorkStore.Api.DTOs.AppSettings;

public class UpdateAppSettingRequest
{
    public string WhatsappNumber { get; set; } = string.Empty;

    public string BusinessName { get; set; } = string.Empty;
}