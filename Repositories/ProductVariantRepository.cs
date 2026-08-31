using Dapper;
using MrPorkStore.Api.Data;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;

namespace MrPorkStore.Api.Repositories;

public class ProductVariantRepository : IProductVariantRepository
{
    private readonly DapperContext _context;

    public ProductVariantRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(
        Guid productId)
    {
        const string sql = """
            SELECT
                id AS Id,
                product_id AS ProductId,
                name AS Name,
                weight_or_size AS WeightOrSize,
                price AS Price,
                stock_quantity AS StockQuantity,
                is_available AS IsAvailable,
                display_order AS DisplayOrder,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM product_variants
            WHERE product_id = @ProductId
            ORDER BY display_order ASC, created_at ASC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<ProductVariant>(
            sql,
            new { ProductId = productId });
    }

    public async Task<ProductVariant?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id AS Id,
                product_id AS ProductId,
                name AS Name,
                weight_or_size AS WeightOrSize,
                price AS Price,
                stock_quantity AS StockQuantity,
                is_available AS IsAvailable,
                display_order AS DisplayOrder,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM product_variants
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<ProductVariant>(
            sql,
            new { Id = id });
    }

    public async Task<Guid> CreateAsync(ProductVariant variant)
    {
        const string sql = """
            INSERT INTO product_variants
            (
                product_id,
                name,
                weight_or_size,
                price,
                stock_quantity,
                is_available,
                display_order
            )
            VALUES
            (
                @ProductId,
                @Name,
                @WeightOrSize,
                @Price,
                @StockQuantity,
                @IsAvailable,
                @DisplayOrder
            )
            RETURNING id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.ExecuteScalarAsync<Guid>(
            sql,
            variant);
    }

    public async Task<bool> UpdateAsync(ProductVariant variant)
    {
        const string sql = """
            UPDATE product_variants
            SET
                name = @Name,
                weight_or_size = @WeightOrSize,
                price = @Price,
                stock_quantity = @StockQuantity,
                is_available = @IsAvailable,
                display_order = @DisplayOrder,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            variant);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = """
            DELETE FROM product_variants
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new { Id = id });

        return rowsAffected > 0;
    }
}