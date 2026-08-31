namespace MrPorkStore.Api.DTOs.PigSubmissions;

public class CreatePigSubmissionRequest
{
    public string FarmerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Location { get; set; }

    public string? PigDetails { get; set; }

    public decimal? Weight { get; set; }

    public decimal? ExpectedPrice { get; set; }
}