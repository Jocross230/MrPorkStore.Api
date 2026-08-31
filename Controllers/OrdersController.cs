using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrPorkStore.Api.DTOs.Orders;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // PUBLIC - Customer places an order
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrderRequest request)
    {
        try
        {
            var orderId =
                await _orderService.CreateAsync(request);

            var order =
                await _orderService.GetByIdAsync(orderId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = orderId },
                order);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
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
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // ADMIN - Get all orders
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders =
            await _orderService.GetAllAsync();

        return Ok(orders);
    }

    // PUBLIC - Get a single order
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var order =
            await _orderService.GetByIdAsync(id);

        if (order is null)
        {
            return NotFound(new
            {
                message = "Order not found."
            });
        }

        return Ok(order);
    }

    // ADMIN - Update order status
    [Authorize]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        UpdateOrderStatusRequest request)
    {
        try
        {
            var updated =
                await _orderService.UpdateStatusAsync(
                    id,
                    request);

            if (!updated)
            {
                return NotFound(new
                {
                    message = "Order not found."
                });
            }

            var order =
                await _orderService.GetByIdAsync(id);

            return Ok(order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}