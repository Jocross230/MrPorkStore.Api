using MrPorkStore.Api.DTOs.Orders;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IProductVariantRepository _variantRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IProductVariantRepository variantRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _variantRepository = variantRepository;
    }

    public async Task<Guid> CreateAsync(
        CreateOrderRequest request)
    {
        ValidateCustomer(request);

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new ArgumentException(
                "Order must contain at least one item.");
        }

        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        foreach (var requestItem in request.Items)
        {
            if (requestItem.Quantity <= 0)
            {
                throw new ArgumentException(
                    "Item quantity must be greater than zero.");
            }

            var product = await _productRepository.GetByIdAsync(
                requestItem.ProductId);

            if (product is null)
            {
                throw new KeyNotFoundException(
                    $"Product {requestItem.ProductId} not found.");
            }

            if (!product.IsActive || !product.IsAvailable)
            {
                throw new InvalidOperationException(
                    $"Product '{product.Name}' is not currently available.");
            }

            decimal unitPrice;
            string? variantLabel = null;

            if (requestItem.ProductVariantId.HasValue)
            {
                var variant = await _variantRepository.GetByIdAsync(
                    requestItem.ProductVariantId.Value);

                if (variant is null)
                {
                    throw new KeyNotFoundException(
                        "Selected product variant not found.");
                }

                if (variant.ProductId != product.Id)
                {
                    throw new ArgumentException(
                        "Selected variant does not belong to the selected product.");
                }

                if (!variant.IsAvailable)
                {
                    throw new InvalidOperationException(
                        $"Product variant '{variant.Name}' is not currently available.");
                }

                if (variant.StockQuantity.HasValue &&
                    requestItem.Quantity > variant.StockQuantity.Value)
                {
                    throw new InvalidOperationException(
                        $"Only {variant.StockQuantity.Value} item(s) are available for '{variant.Name}'.");
                }

                unitPrice = variant.Price;

                variantLabel = string.IsNullOrWhiteSpace(
                    variant.WeightOrSize)
                    ? variant.Name
                    : $"{variant.Name} - {variant.WeightOrSize}";
            }
            else
            {
                if (!product.Price.HasValue)
                {
                    throw new InvalidOperationException(
                        $"Product '{product.Name}' does not have a price.");
                }

                if (product.StockQuantity.HasValue &&
                    requestItem.Quantity > product.StockQuantity.Value)
                {
                    throw new InvalidOperationException(
                        $"Only {product.StockQuantity.Value} item(s) are available for '{product.Name}'.");
                }

                unitPrice = product.Price.Value;

                variantLabel = product.WeightOrSize;
            }

            var subtotal = unitPrice * requestItem.Quantity;

            totalAmount += subtotal;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductVariantId = requestItem.ProductVariantId,
                ProductName = product.Name,
                VariantLabel = variantLabel,
                UnitPrice = unitPrice,
                Quantity = requestItem.Quantity
            });
        }

        var order = new Order
        {
            CustomerName = request.CustomerName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email)
                ? null
                : request.Email.Trim(),
            DeliveryAddress = string.IsNullOrWhiteSpace(
                request.DeliveryAddress)
                ? null
                : request.DeliveryAddress.Trim(),
            Status = "NEW",
            TotalAmount = totalAmount
        };

        return await _orderRepository.CreateAsync(
            order,
            orderItems);
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return null;
        }

        var items = await _orderRepository.GetItemsByOrderIdAsync(
            id);

        return MapToResponse(order, items);
    }

    public async Task<IEnumerable<OrderResponse>> GetAllAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        var responses = new List<OrderResponse>();

        foreach (var order in orders)
        {
            var items =
                await _orderRepository.GetItemsByOrderIdAsync(order.Id);

            responses.Add(MapToResponse(order, items));
        }

        return responses;
    }

    public async Task<bool> UpdateStatusAsync(
        Guid id,
        UpdateOrderStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            throw new ArgumentException(
                "Order status is required.");
        }

        var status = request.Status.Trim().ToUpperInvariant();

        var allowedStatuses = new[]
        {
            "NEW",
            "CONFIRMED",
            "PROCESSING",
            "COMPLETED",
            "CANCELLED"
        };

        if (!allowedStatuses.Contains(status))
        {
            throw new ArgumentException(
                "Invalid status. Allowed values are: " +
                "NEW, CONFIRMED, PROCESSING, COMPLETED, CANCELLED.");
        }

        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return false;
        }

        return await _orderRepository.UpdateStatusAsync(
            id,
            status);
    }

    private static void ValidateCustomer(
        CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            throw new ArgumentException(
                "Customer name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            throw new ArgumentException(
                "Phone number is required.");
        }
    }

    private static OrderResponse MapToResponse(
        Order order,
        IEnumerable<OrderItem> items)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            PhoneNumber = order.PhoneNumber,
            Email = order.Email,
            DeliveryAddress = order.DeliveryAddress,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = items.Select(item => new OrderItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductName,
                VariantLabel = item.VariantLabel,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList()
        };
    }
}