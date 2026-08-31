using Dapper;
using MrPorkStore.Api.Data;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;

namespace MrPorkStore.Api.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly DapperContext _context;

    public AdminRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Admin?> GetByEmailAsync(string email)
    {
        const string sql = """
            SELECT
                id AS Id,
                email AS Email,
                password_hash AS PasswordHash,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM admins
            WHERE LOWER(email) = LOWER(@Email);
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Admin>(
            sql,
            new { Email = email });
    }

    public async Task<Admin?> GetByIdAsync(Guid id)
    {
        const string sql = """
            SELECT
                id AS Id,
                email AS Email,
                password_hash AS PasswordHash,
                created_at AS CreatedAt,
                updated_at AS UpdatedAt
            FROM admins
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<Admin>(
            sql,
            new { Id = id });
    }

    public async Task<Guid> CreateAsync(Admin admin)
    {
        const string sql = """
            INSERT INTO admins (
                id,
                email,
                password_hash,
                created_at,
                updated_at
            )
            VALUES (
                @Id,
                @Email,
                @PasswordHash,
                CURRENT_TIMESTAMP,
                CURRENT_TIMESTAMP
            )
            RETURNING id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.ExecuteScalarAsync<Guid>(sql, admin);
    }

    public async Task UpdatePasswordAsync(
        Guid adminId,
        string passwordHash)
    {
        const string sql = """
            UPDATE admins
            SET
                password_hash = @PasswordHash,
                updated_at = CURRENT_TIMESTAMP
            WHERE id = @AdminId;
            """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new
            {
                AdminId = adminId,
                PasswordHash = passwordHash
            });
    }

    public async Task CreatePasswordResetTokenAsync(
        PasswordResetToken resetToken)
    {
        const string sql = """
            INSERT INTO password_reset_tokens (
                id,
                admin_id,
                token_hash,
                expires_at,
                used_at,
                created_at
            )
            VALUES (
                @Id,
                @AdminId,
                @TokenHash,
                @ExpiresAt,
                NULL,
                CURRENT_TIMESTAMP
            );
            """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(sql, resetToken);
    }

    public async Task<PasswordResetToken?>
        GetValidPasswordResetTokenAsync(string tokenHash)
    {
        const string sql = """
            SELECT
                id AS Id,
                admin_id AS AdminId,
                token_hash AS TokenHash,
                expires_at AS ExpiresAt,
                used_at AS UsedAt,
                created_at AS CreatedAt
            FROM password_reset_tokens
            WHERE token_hash = @TokenHash
              AND used_at IS NULL
              AND expires_at > CURRENT_TIMESTAMP
            ORDER BY created_at DESC
            LIMIT 1;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<PasswordResetToken>(
            sql,
            new { TokenHash = tokenHash });
    }

    public async Task MarkPasswordResetTokenAsUsedAsync(Guid tokenId)
    {
        const string sql = """
            UPDATE password_reset_tokens
            SET used_at = CURRENT_TIMESTAMP
            WHERE id = @TokenId;
            """;

        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(
            sql,
            new { TokenId = tokenId });
    }
}