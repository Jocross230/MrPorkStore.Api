using Dapper;
using MrPorkStore.Api.Data;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;

namespace MrPorkStore.Api.Repositories;

public class AppSettingRepository : IAppSettingRepository
{
    private readonly DapperContext _context;

    public AppSettingRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<AppSetting?> GetAsync()
    {
        const string sql = """
            SELECT
                id AS Id,
                whatsapp_number AS WhatsappNumber,
                business_name AS BusinessName,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM app_settings
            ORDER BY created_at ASC
            LIMIT 1;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<AppSetting>(
            sql);
    }

    public async Task<bool> UpdateAsync(AppSetting setting)
    {
        const string sql = """
            UPDATE app_settings
            SET
                whatsapp_number = @WhatsappNumber,
                business_name = @BusinessName,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            setting);

        return rowsAffected > 0;
    }
}