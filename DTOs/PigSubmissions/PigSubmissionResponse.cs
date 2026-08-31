using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.DTOs.PigSubmissions;

public class PigSubmissionResponse
{
    public Guid Id { get; set; }

    public string FarmerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Location { get; set; }

    public string? PigDetails { get; set; }

    public decimal? Weight { get; set; }

    public decimal? ExpectedPrice { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public IEnumerable<PigSubmissionImage> Images { get; set; }
        = Enumerable.Empty<PigSubmissionImage>();
}