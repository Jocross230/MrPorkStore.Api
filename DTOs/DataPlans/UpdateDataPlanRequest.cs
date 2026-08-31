namespace MrPorkStore.Api.DTOs.DataPlans;

public class UpdateDataPlanRequest
{
    public Guid NetworkId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DataSize { get; set; } = string.Empty;
    public string? Validity { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}