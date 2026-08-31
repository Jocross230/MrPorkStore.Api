using MrPorkStore.Api.DTOs.Orders;

namespace MrPorkStore.Api.Services.Interfaces;

public interface IOrderService
{
    Task<Guid> CreateAsync(CreateOrderRequest request);

    Task<OrderResponse?> GetByIdAsync(Guid id);

    Task<IEnumerable<OrderResponse>> GetAllAsync();

    Task<bool> UpdateStatusAsync(
        Guid id,
        UpdateOrderStatusRequest request);
}