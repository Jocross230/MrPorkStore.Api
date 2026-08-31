using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrPorkStore.Api.DTOs.PigSubmissionImages;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Controllers;

[ApiController]
[Route("api")]
public class PigSubmissionImagesController : ControllerBase
{
    private readonly IPigSubmissionImageService _imageService;
    private readonly IPigSubmissionImageRepository _imageRepository;

    public PigSubmissionImagesController(
        IPigSubmissionImageService imageService,
        IPigSubmissionImageRepository imageRepository)
    {
        _imageService = imageService;
        _imageRepository = imageRepository;
    }

    // ADMIN ONLY - Get images for a pig submission
    [Authorize]
    [HttpGet("admin/pig-submissions/{submissionId:guid}/images")]
    public async Task<IActionResult> GetBySubmissionId(
        Guid submissionId)
    {
        try
        {
            var images =
                await _imageService.GetBySubmissionIdAsync(
                    submissionId);

            return Ok(images);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    // ADMIN ONLY - Upload image
    [Authorize]
    [HttpPost("admin/pig-submissions/{submissionId:guid}/images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
    Guid submissionId,
    [FromForm] UploadPigSubmissionImageRequest request)
    {
        try
        {
            var result = await _imageService.UploadAsync(
                submissionId,
                request.File);

            return CreatedAtAction(
                nameof(GetBySubmissionId),
                new { submissionId },
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

    // ADMIN ONLY - Delete image
    [Authorize]
    [HttpDelete("admin/pig-submission-images/{imageId:guid}")]
    public async Task<IActionResult> Delete(Guid imageId)
    {
        try
        {
            var deleted =
                await _imageService.DeleteAsync(imageId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Pig submission image not found."
                });
            }

            return Ok(new
            {
                message =
                    "Pig submission image deleted successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, new
            {
                message = ex.Message
            });
        }
    }
}