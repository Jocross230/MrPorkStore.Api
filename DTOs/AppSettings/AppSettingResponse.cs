namespace MrPorkStore.Api.DTOs.AppSettings;

public class AppSettingResponse
{
    public Guid Id { get; set; }

    public string WhatsappNumber { get; set; } = string.Empty;

    public string BusinessName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}