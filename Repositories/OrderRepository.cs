using Dapper;
using MrPorkStore.Api.Data;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;

namespace MrPorkStore.Api.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DapperContext _context;

    public OrderRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(
        Order order,
        IEnumerable<OrderItem> items)
    {
        const string orderSql = """
            INSERT INTO orders
            (
                customer_name,
                phone_number,
                email,
                delivery_address,
                status,
                total_amount
            )
            VALUES
            (
                @CustomerName,
                @PhoneNumber,
                @Email,
                @DeliveryAddress,
                @Status,
                @TotalAmount
            )
            RETURNING id;
            """;

        const string itemSql = """
            INSERT INTO order_items
            (
                order_id,
                product_id,
                product_variant_id,
                product_name,
                variant_label,
                unit_price,
                quantity
            )
            VALUES
            (
                @OrderId,
                @ProductId,
                @ProductVariantId,
                @ProductName,
                @VariantLabel,
                @UnitPrice,
                @Quantity
            );
            """;

        using var connection = _context.CreateConnection();

        await connection.OpenAsync();

        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var orderId = await connection.ExecuteScalarAsync<Guid>(
                orderSql,
                order,
                transaction);

            foreach (var item in items)
            {
                item.OrderId = orderId;

                await connection.ExecuteAsync(
                    itemSql,
                    item,
                    transaction);
            }

            await transaction.CommitAsync();

            return orderId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id AS Id,
                customer_name AS CustomerName,
                phone_number AS PhoneNumber,
                email AS Email,
                delivery_address AS DeliveryAddress,
                status AS Status,
                total_amount AS TotalAmount,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM orders
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Order>(
            sql,
            new { Id = id });
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        const string sql = """
            SELECT
                id AS Id,
                customer_name AS CustomerName,
                phone_number AS PhoneNumber,
                email AS Email,
                delivery_address AS DeliveryAddress,
                status AS Status,
                total_amount AS TotalAmount,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM orders
            ORDER BY created_at DESC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<Order>(sql);
    }

    public async Task<bool> UpdateStatusAsync(
        Guid id,
        string status)
    {
        const string sql = """
            UPDATE orders
            SET
                status = @Status,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                Status = status
            });

        return rowsAffected > 0;
    }

    public async Task<IEnumerable<OrderItem>> GetItemsByOrderIdAsync(
        Guid orderId)
    {
        const string sql = """
            SELECT
                id AS Id,
                order_id AS OrderId,
                product_id AS ProductId,
                product_variant_id AS ProductVariantId,
                product_name AS ProductName,
                variant_label AS VariantLabel,
                unit_price AS UnitPrice,
                quantity AS Quantity,
                created_at AS CreatedAt
            FROM order_items
            WHERE order_id = @OrderId
            ORDER BY created_at ASC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<OrderItem>(
            sql,
            new { OrderId = orderId });
    }
}