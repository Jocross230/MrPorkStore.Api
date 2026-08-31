using Dapper;
using MrPorkStore.Api.Data;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;

namespace MrPorkStore.Api.Repositories;

public class PigSubmissionImageRepository
    : IPigSubmissionImageRepository
{
    private readonly DapperContext _context;

    public PigSubmissionImageRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<PigSubmissionImage> CreateAsync(
    PigSubmissionImage image)
    {
        const string sql = """
        INSERT INTO pig_submission_images
        (
            pig_submission_id,
            image_url,
            public_id
        )
        VALUES
        (
            @PigSubmissionId,
            @ImageUrl,
            @PublicId
        )
        RETURNING
            id AS Id,
            pig_submission_id AS PigSubmissionId,
            image_url AS ImageUrl,
            public_id AS PublicId,
            created_at AS CreatedAt;
        """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleAsync<PigSubmissionImage>(
            sql,
            image);
    }

    public async Task<IEnumerable<PigSubmissionImage>>
        GetBySubmissionIdAsync(Guid submissionId)
    {
        const string sql = """
            SELECT
                id AS Id,
                pig_submission_id AS PigSubmissionId,
                image_url AS ImageUrl,
                public_id AS PublicId,
                created_at AS CreatedAt
            FROM pig_submission_images
            WHERE pig_submission_id = @SubmissionId
            ORDER BY created_at ASC;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<PigSubmissionImage>(
            sql,
            new { SubmissionId = submissionId });
    }

    public async Task<PigSubmissionImage?> GetByIdAsync(
        Guid id)
    {
        const string sql = """
            SELECT
                id AS Id,
                pig_submission_id AS PigSubmissionId,
                image_url AS ImageUrl,
                public_id AS PublicId,
                created_at AS CreatedAt
            FROM pig_submission_images
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<PigSubmissionImage>(
            sql,
            new { Id = id });
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        const string sql = """
            DELETE FROM pig_submission_images
            WHERE id = @Id;
            """;

        using var connection = _context.CreateConnection();

        var rowsAffected = await connection.ExecuteAsync(
            sql,
            new { Id = id });

        return rowsAffected > 0;
    }
}