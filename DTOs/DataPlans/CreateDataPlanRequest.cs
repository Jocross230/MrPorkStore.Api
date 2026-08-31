namespace MrPorkStore.Api.DTOs.DataPlans;

public class CreateDataPlanRequest
{
    public Guid NetworkId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DataSize { get; set; } = string.Empty;
    public string? Validity { get; set; }
    public decimal Price { get; set; }
}