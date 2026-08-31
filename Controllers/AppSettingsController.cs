using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrPorkStore.Api.DTOs.AppSettings;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Controllers;

[ApiController]
[Route("api/app-settings")]
public class AppSettingsController : ControllerBase
{
    private readonly IAppSettingService _service;

    public AppSettingsController(
        IAppSettingService service)
    {
        _service = service;
    }

    // PUBLIC - Frontend needs WhatsApp/business information
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var setting = await _service.GetAsync();

        if (setting is null)
        {
            return NotFound(new
            {
                message = "Application settings have not been configured."
            });
        }

        return Ok(setting);
    }

    // ADMIN ONLY - Update WhatsApp/business information
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update(
        UpdateAppSettingRequest request)
    {
        try
        {
            var setting = await _service.UpdateAsync(request);

            return Ok(setting);
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
}