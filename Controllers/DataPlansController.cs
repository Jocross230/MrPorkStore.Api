using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrPorkStore.Api.DTOs.DataPlans;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Controllers;

[ApiController]
[Route("api/data-plans")]
public class DataPlansController : ControllerBase
{
    private readonly IDataPlanService _service;

    public DataPlansController(IDataPlanService service)
    {
        _service = service;
    }

    // PUBLIC — frontend gets all available plans
    [HttpGet]
    public async Task<IActionResult> GetAvailable()
    {
        var plans = await _service.GetAvailableAsync();

        return Ok(plans);
    }

    // PUBLIC — get plans for one active network
    [HttpGet("network/{networkId:guid}")]
    public async Task<IActionResult> GetByNetworkId(Guid networkId)
    {
        var plans = await _service.GetByNetworkIdAsync(networkId);

        return Ok(plans);
    }

    // PUBLIC — get one available plan
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var plan = await _service.GetByIdAsync(id);

        if (plan is null || !plan.IsAvailable)
        {
            return NotFound(new
            {
                message = "Data plan not found."
            });
        }

        return Ok(plan);
    }

    // ADMIN — view all plans including unavailable ones
    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var plans = await _service.GetAllAsync();

        return Ok(plans);
    }

    // ADMIN
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateDataPlanRequest request)
    {
        try
        {
            var id = await _service.CreateAsync(request);

            var plan = await _service.GetByIdAsync(id);

            return CreatedAtAction(
                nameof(GetById),
                new { id },
                plan);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // ADMIN
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateDataPlanRequest request)
    {
        try
        {
            var updated = await _service.UpdateAsync(
                id,
                request);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Data plan not found."
                });
            }

            var plan = await _service.GetByIdAsync(id);

            return Ok(plan);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    // ADMIN
    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Data plan not found."
            });
        }

        return Ok(new
        {
            message = "Data plan deleted successfully."
        });
    }
}