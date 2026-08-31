namespace MrPorkStore.Api.DTOs.DataNetworks;

public class CreateDataNetworkRequest
{
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
}