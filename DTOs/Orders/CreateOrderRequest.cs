namespace MrPorkStore.Api.DTOs.Orders;

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? DeliveryAddress { get; set; }

    public List<CreateOrderItemRequest> Items { get; set; } = new();
}