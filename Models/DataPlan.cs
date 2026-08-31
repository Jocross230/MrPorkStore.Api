namespace MrPorkStore.Api.Models;

public class DataPlan
{
    public Guid Id { get; set; }
    public Guid NetworkId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DataSize { get; set; } = string.Empty;
    public string? Validity { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}