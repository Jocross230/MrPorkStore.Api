namespace MrPorkStore.Api.Models;

public class AppSetting
{
    public Guid Id { get; set; }

    public string WhatsappNumber { get; set; } = string.Empty;

    public string BusinessName { get; set; } = "Mr.Pork Store";

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}