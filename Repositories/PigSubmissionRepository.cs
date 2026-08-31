using Dapper;
using MrPorkStore.Api.Data;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;

namespace MrPorkStore.Api.Repositories;

public class PigSubmissionRepository : IPigSubmissionRepository
{
    private readonly DapperContext _context;

    public PigSubmissionRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateAsync(PigSubmission submission)
    {
        const string sql = """
        INSERT INTO pig_submissions
        (
            farmer_name,
            phone_number,
            email,
            location,
            pig_details,
            weight,
            expected_price,
            status
        )
        VALUES
        (
            @FarmerName,
            @PhoneNumber,
            @Email,
            @Location,
            @PigDetails,
            @Weight,
            @ExpectedPrice,
            @Status
        )
        RETURNING id;
        """;

        using var connection = _context.CreateConnection();

        return await connection.ExecuteScalarAsync<Guid>(
            sql,
            submission);
    }

    public async Task<IEnumerable<PigSubmission>> GetAllAsync()
    {
        const string sql = """
    SELECT
        id AS Id,
        farmer_name AS FarmerName,
        phone_number AS PhoneNumber,
        email AS Email,
        location AS Location,
        pig_details AS PigDetails,
        weight AS Weight,
        expected_price AS ExpectedPrice,
        status AS Status,
        created_at AS CreatedAt,
        updated_at AS UpdatedAt
    FROM pig_submissions
    ORDER BY created_at DESC;
    """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<PigSubmission>(sql);
    }

    public async Task<PigSubmission?> GetByIdAsync(Guid id)
    {
        const string sql = """
    SELECT
        id AS Id,
        farmer_name AS FarmerName,
        phone_number AS PhoneNumber,
        email AS Email,
        location AS Location,
        pig_details AS PigDetails,
        weight AS Weight,
        expected_price AS ExpectedPrice,
        status AS Status,
        created_at AS CreatedAt,
        updated_at AS UpdatedAt
    FROM pig_submissions
    WHERE id = @Id;
    """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<PigSubmission>(
            sql,
            new { Id = id });
    }

    public async Task<bool> UpdateStatusAsync(Guid id, string status)
    {
        const string sql = """
            UPDATE pig_submissions
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

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = """
            DELETE FROM pig_submissions
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new { Id = id });

        return rowsAffected > 0;
    }
}