using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrPorkStore.Api.DTOs.ProductImages;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Controllers;

[ApiController]
[Route("api")]
public class ProductImagesController : ControllerBase
{
    private readonly IProductImageService _imageService;
    private readonly IProductImageRepository _imageRepository;

    public ProductImagesController(
        IProductImageService imageService,
        IProductImageRepository imageRepository)
    {
        _imageService = imageService;
        _imageRepository = imageRepository;
    }

    // PUBLIC - Frontend needs to display product images
    [HttpGet("products/{productId:guid}/images")]
    public async Task<IActionResult> GetByProductId(Guid productId)
    {
        var images = await _imageRepository.GetByProductIdAsync(productId);

        return Ok(images);
    }

    // ADMIN ONLY - Upload a product image
    [Authorize]
    [HttpPost("products/{productId:guid}/images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
    Guid productId,
    [FromForm] UploadProductImageRequest request)
    {
        try
        {
            var result = await _imageService.UploadAsync(
                productId,
                request.File);

            return CreatedAtAction(
                nameof(GetByProductId),
                new { productId },
                result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    // ADMIN ONLY - Delete an image
    [Authorize]
    [HttpDelete("product-images/{imageId:guid}")]
    public async Task<IActionResult> Delete(Guid imageId)
    {
        try
        {
            var deleted = await _imageService.DeleteAsync(imageId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Product image not found."
                });
            }

            return Ok(new
            {
                message = "Product image deleted successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}