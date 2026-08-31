namespace MrPorkStore.Api.Models;

public class PigSubmissionImage
{
    public Guid Id { get; set; }

    public Guid PigSubmissionId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public string? PublicId { get; set; }

    public DateTime CreatedAt { get; set; }
}