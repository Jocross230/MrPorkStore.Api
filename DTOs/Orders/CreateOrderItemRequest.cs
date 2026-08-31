namespace MrPorkStore.Api.DTOs.Orders;

public class CreateOrderItemRequest
{
    public Guid ProductId { get; set; }

    public Guid? ProductVariantId { get; set; }

    public int Quantity { get; set; }
}