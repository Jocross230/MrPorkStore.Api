using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Guid> CreateAsync(
        Order order,
        IEnumerable<OrderItem> items);

    Task<Order?> GetByIdAsync(Guid id);

    Task<IEnumerable<Order>> GetAllAsync();

    Task<bool> UpdateStatusAsync(
        Guid id,
        string status);

    Task<IEnumerable<OrderItem>> GetItemsByOrderIdAsync(
        Guid orderId);
}