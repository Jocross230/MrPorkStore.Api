using Microsoft.AspNetCore.Http;

namespace MrPorkStore.Api.DTOs.ProductImages;

public class UploadProductImageRequest
{
    public IFormFile File { get; set; } = null!;
}