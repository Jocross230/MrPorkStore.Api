using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Repositories.Interfaces;

public interface IPigSubmissionImageRepository
{
    Task<PigSubmissionImage> CreateAsync(
    PigSubmissionImage image);

    Task<IEnumerable<PigSubmissionImage>> GetBySubmissionIdAsync(
        Guid submissionId);

    Task<PigSubmissionImage?> GetByIdAsync(Guid id);

    Task<bool> DeleteAsync(Guid id);
}