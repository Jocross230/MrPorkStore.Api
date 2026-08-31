using Dapper;
using MrPorkStore.Api.Data;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;

namespace MrPorkStore.Api.Repositories;

public class DataPlanRepository : IDataPlanRepository
{
    private readonly DapperContext _context;

    public DataPlanRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DataPlan>> GetAllAsync()
    {
        const string sql = """
            SELECT
                id AS Id,
                network_id AS NetworkId,
                name AS Name,
                data_size AS DataSize,
                validity AS Validity,
                price AS Price,
                is_available AS IsAvailable,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM data_plans
            ORDER BY price ASC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<DataPlan>(sql);
    }

    public async Task<IEnumerable<DataPlan>> GetAvailableAsync()
    {
        const string sql = """
            SELECT
                id AS Id,
                network_id AS NetworkId,
                name AS Name,
                data_size AS DataSize,
                validity AS Validity,
                price AS Price,
                is_available AS IsAvailable,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM data_plans
            WHERE is_available = TRUE
            ORDER BY price ASC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<DataPlan>(sql);
    }

    public async Task<IEnumerable<DataPlan>> GetByNetworkIdAsync(
        Guid networkId)
    {
        const string sql = """
            SELECT
                id AS Id,
                network_id AS NetworkId,
                name AS Name,
                data_size AS DataSize,
                validity AS Validity,
                price AS Price,
                is_available AS IsAvailable,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM data_plans
            WHERE network_id = @NetworkId
              AND is_available = TRUE
            ORDER BY price ASC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<DataPlan>(
            sql,
            new { NetworkId = networkId });
    }

    public async Task<DataPlan?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id AS Id,
                network_id AS NetworkId,
                name AS Name,
                data_size AS DataSize,
                validity AS Validity,
                price AS Price,
                is_available AS IsAvailable,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM data_plans
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<DataPlan>(
            sql,
            new { Id = id });
    }

    public async Task<Guid> CreateAsync(DataPlan plan)
    {
        const string sql = """
            INSERT INTO data_plans
            (
                network_id,
                name,
                data_size,
                validity,
                price
            )
            VALUES
            (
                @NetworkId,
                @Name,
                @DataSize,
                @Validity,
                @Price
            )
            RETURNING id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.ExecuteScalarAsync<Guid>(
            sql,
            plan);
    }

    public async Task<bool> UpdateAsync(DataPlan plan)
    {
        const string sql = """
            UPDATE data_plans
            SET
                network_id = @NetworkId,
                name = @Name,
                data_size = @DataSize,
                validity = @Validity,
                price = @Price,
                is_available = @IsAvailable,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            plan);

        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = """
            DELETE FROM data_plans
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new { Id = id });

        return rowsAffected > 0;
    }
}