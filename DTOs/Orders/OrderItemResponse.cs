namespace MrPorkStore.Api.DTOs.Orders;

public class OrderItemResponse
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid? ProductVariantId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string? VariantLabel { get; set; }

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal Subtotal =>
        UnitPrice * Quantity;
}