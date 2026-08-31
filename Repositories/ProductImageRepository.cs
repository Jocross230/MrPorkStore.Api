using Dapper;
using MrPorkStore.Api.Data;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;

namespace MrPorkStore.Api.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly DapperContext _context;

    public ProductImageRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(
        Guid productId)
    {
        const string sql = """
            SELECT
                id AS Id,
                product_id AS ProductId,
                image_url AS ImageUrl,
                public_id AS PublicId,
                display_order AS DisplayOrder,
                is_primary AS IsPrimary,
                created_at AS CreatedAt
            FROM product_images
            WHERE product_id = @ProductId
            ORDER BY is_primary DESC, display_order ASC, created_at ASC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<ProductImage>(
            sql,
            new { ProductId = productId });
    }

    public async Task<ProductImage?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id AS Id,
                product_id AS ProductId,
                image_url AS ImageUrl,
                public_id AS PublicId,
                display_order AS DisplayOrder,
                is_primary AS IsPrimary,
                created_at AS CreatedAt
            FROM product_images
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<ProductImage>(
            sql,
            new { Id = id });
    }

    public async Task<Guid> CreateAsync(ProductImage image)
    {
        const string sql = """
            INSERT INTO product_images
            (
                product_id,
                image_url,
                public_id,
                display_order,
                is_primary
            )
            VALUES
            (
                @ProductId,
                @ImageUrl,
                @PublicId,
                @DisplayOrder,
                @IsPrimary
            )
            RETURNING id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.ExecuteScalarAsync<Guid>(sql, image);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = """
            DELETE FROM product_images
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new { Id = id });

        return rowsAffected > 0;
    }

    public async Task<bool> SetPrimaryAsync(
        Guid productId,
        Guid imageId)
    {
        using var connection = _context.CreateConnection();

        // First remove primary status from every image
        // belonging to this product.
        const string resetSql = """
            UPDATE product_images
            SET is_primary = FALSE
            WHERE product_id = @ProductId;
            """;

        await connection.ExecuteAsync(
            resetSql,
            new { ProductId = productId });

        // Then make the selected image primary.
        const string setPrimarySql = """
            UPDATE product_images
            SET is_primary = TRUE
            WHERE id = @ImageId
              AND product_id = @ProductId;
            """;

        var rowsAffected = await connection.ExecuteAsync(
            setPrimarySql,
            new
            {
                ImageId = imageId,
                ProductId = productId
            });

        return rowsAffected > 0;
    }
}