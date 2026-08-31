namespace MrPorkStore.Api.DTOs.Orders;

public class OrderResponse
{
    public Guid Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? DeliveryAddress { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<OrderItemResponse> Items { get; set; } = new();
}