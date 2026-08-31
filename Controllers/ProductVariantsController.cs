using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrPorkStore.Api.DTOs.ProductVariants;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Controllers;

[ApiController]
public class ProductVariantsController : ControllerBase
{
    private readonly IProductVariantService _variantService;

    public ProductVariantsController(
        IProductVariantService variantService)
    {
        _variantService = variantService;
    }

    // PUBLIC - Customers can view variants for a product
    [HttpGet("api/products/{productId:guid}/variants")]
    public async Task<IActionResult> GetByProductId(Guid productId)
    {
        var variants =
            await _variantService.GetByProductIdAsync(productId);

        return Ok(variants);
    }

    // ADMIN ONLY - Add a variant to a product
    [Authorize]
    [HttpPost("api/products/{productId:guid}/variants")]
    public async Task<IActionResult> Create(
        Guid productId,
        CreateProductVariantRequest request)
    {
        var variant =
            await _variantService.CreateAsync(productId, request);

        if (variant is null)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        return Created(
            $"/api/product-variants/{variant.Id}",
            variant);
    }

    // ADMIN ONLY - Update a variant
    [Authorize]
    [HttpPut("api/product-variants/{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        UpdateProductVariantRequest request)
    {
        var updated =
            await _variantService.UpdateAsync(id, request);

        if (!updated)
        {
            return NotFound(new
            {
                message = "Product variant not found."
            });
        }

        return Ok(new
        {
            message = "Product variant updated successfully."
        });
    }

    // ADMIN ONLY - Delete a variant
    [Authorize]
    [HttpDelete("api/product-variants/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted =
            await _variantService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Product variant not found."
            });
        }

        return Ok(new
        {
            message = "Product variant deleted successfully."
        });
    }
}