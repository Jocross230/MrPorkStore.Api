using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrPorkStore.Api.DTOs.PigSubmissions;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Controllers;

[ApiController]
[Route("api")]
public class PigSubmissionsController : ControllerBase
{
    private readonly IPigSubmissionService _service;

    public PigSubmissionsController(IPigSubmissionService service)
    {
        _service = service;
    }

    // PUBLIC - Farmers/customers can submit their pig details
    [HttpPost("sell-your-pig")]
    public async Task<IActionResult> Create(
        [FromBody] CreatePigSubmissionRequest request)
    {
        try
        {
            var result = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
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

    // ADMIN ONLY - View all submissions
    [Authorize]
    [HttpGet("admin/pig-submissions")]
    public async Task<IActionResult> GetAll()
    {
        var results = await _service.GetAllAsync();

        return Ok(results);
    }

    // ADMIN ONLY - View one submission
    [Authorize]
    [HttpGet("admin/pig-submissions/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound(new
            {
                message = "Pig submission not found."
            });
        }

        return Ok(result);
    }

    // ADMIN ONLY - Update submission status
    [Authorize]
    [HttpPut("admin/pig-submissions/{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdatePigSubmissionStatusRequest request)
    {
        try
        {
            var updated = await _service.UpdateStatusAsync(id, request);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Pig submission not found."
                });
            }

            return Ok(new
            {
                message = "Pig submission status updated successfully."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // ADMIN ONLY - Delete submission
    [Authorize]
    [HttpDelete("admin/pig-submissions/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Pig submission not found."
            });
        }

        return Ok(new
        {
            message = "Pig submission deleted successfully."
        });
    }
}