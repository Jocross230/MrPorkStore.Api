using Microsoft.AspNetCore.Http;

namespace MrPorkStore.Api.DTOs.PigSubmissionImages;

public class UploadPigSubmissionImageRequest
{
    public IFormFile File { get; set; } = null!;
}