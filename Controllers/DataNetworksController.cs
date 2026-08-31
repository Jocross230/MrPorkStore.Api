using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrPorkStore.Api.DTOs.DataNetworks;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Controllers;

[ApiController]
[Route("api/data-networks")]
public class DataNetworksController : ControllerBase
{
    private readonly IDataNetworkService _service;

    public DataNetworksController(IDataNetworkService service)
    {
        _service = service;
    }

    // PUBLIC — frontend can display active networks
    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var networks = await _service.GetActiveAsync();

        return Ok(networks);
    }

    // ADMIN — view all networks including inactive ones
    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var networks = await _service.GetAllAsync();

        return Ok(networks);
    }

    // PUBLIC — get one active network
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var network = await _service.GetByIdAsync(id);

        if (network is null || !network.IsActive)
        {
            return NotFound(new
            {
                message = "Data network not found."
            });
        }

        return Ok(network);
    }

    // ADMIN
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateDataNetworkRequest request)
    {
        try
        {
            var id = await _service.CreateAsync(request);

            var network = await _service.GetByIdAsync(id);

            return CreatedAtAction(
                nameof(GetById),
                new { id },
                network);
        }
        catch (ArgumentException ex)
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
        UpdateDataNetworkRequest request)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Data network not found."
                });
            }

            var network = await _service.GetByIdAsync(id);

            return Ok(network);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
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
                message = "Data network not found."
            });
        }

        return Ok(new
        {
            message = "Data network deleted successfully."
        });
    }
}