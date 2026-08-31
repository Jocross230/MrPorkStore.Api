using Dapper;
using MrPorkStore.Api.Data;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;

namespace MrPorkStore.Api.Repositories;

public class DataNetworkRepository : IDataNetworkRepository
{
    private readonly DapperContext _context;

    public DataNetworkRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DataNetwork>> GetAllAsync()
    {
        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                logo_url AS LogoUrl,
                is_active AS IsActive,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM data_networks
            ORDER BY name ASC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<DataNetwork>(sql);
    }

    public async Task<IEnumerable<DataNetwork>> GetActiveAsync()
    {
        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                logo_url AS LogoUrl,
                is_active AS IsActive,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM data_networks
            WHERE is_active = TRUE
            ORDER BY name ASC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<DataNetwork>(sql);
    }

    public async Task<DataNetwork?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id AS Id,
                name AS Name,
                logo_url AS LogoUrl,
                is_active AS IsActive,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM data_networks
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<DataNetwork>(
            sql,
            new { Id = id });
    }

    public async Task<Guid> CreateAsync(DataNetwork network)
    {
        const string sql = """
            INSERT INTO data_networks
            (
                name,
                logo_url
            )
            VALUES
            (
                @Name,
                @LogoUrl
            )
            RETURNING id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.ExecuteScalarAsync<Guid>(
            sql,
            network);
    }

    public async Task<bool> UpdateAsync(DataNetwork network)
    {
        const string sql = """
            UPDATE data_networks
            SET
                name = @Name,
                logo_url = @LogoUrl,
                is_active = @IsActive,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            network);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = """
            DELETE FROM data_networks
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new { Id = id });

        return rowsAffected > 0;
    }
}