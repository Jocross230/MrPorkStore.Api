namespace MrPorkStore.Api.DTOs.DataNetworks;

public class UpdateDataNetworkRequest
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
}