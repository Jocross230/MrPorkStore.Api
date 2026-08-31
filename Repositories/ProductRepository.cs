using Dapper;
using MrPorkStore.Api.Data;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;

namespace MrPorkStore.Api.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DapperContext _context;

    public ProductRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                description AS Description,
                category AS Category,
                product_type AS ProductType,
                weight_or_size AS WeightOrSize,
                price AS Price,
                stock_quantity AS StockQuantity,
                is_available AS IsAvailable,
                is_active AS IsActive,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM products
            ORDER BY created_at DESC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<Product>(sql);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                description AS Description,
                category AS Category,
                product_type AS ProductType,
                weight_or_size AS WeightOrSize,
                price AS Price,
                stock_quantity AS StockQuantity,
                is_available AS IsAvailable,
                is_active AS IsActive,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM products
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Product>(
            sql,
            new { Id = id });
    }

    public async Task<Guid> CreateAsync(Product product)
    {
        const string sql = """
            INSERT INTO products
            (
                name,
                description,
                category,
                product_type,
                weight_or_size,
                price,
                stock_quantity,
                is_available,
                is_active
            )
            VALUES
            (
                @Name,
                @Description,
                @Category,
                @ProductType,
                @WeightOrSize,
                @Price,
                @StockQuantity,
                @IsAvailable,
                @IsActive
            )
            RETURNING id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.ExecuteScalarAsync<Guid>(
            sql,
            product);
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        const string sql = """
            UPDATE products
            SET
                name = @Name,
                description = @Description,
                category = @Category,
                product_type = @ProductType,
                weight_or_size = @WeightOrSize,
                price = @Price,
                stock_quantity = @StockQuantity,
                is_available = @IsAvailable,
                is_active = @IsActive,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(sql, product);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = """
        UPDATE products
        SET
            is_active = false,
            is_available = false,
            updated_at = CURRENT_TIMESTAMP
        WHERE id = @Id
          AND is_active = true;
        """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new { Id = id });

        return rowsAffected > 0;
    }
}