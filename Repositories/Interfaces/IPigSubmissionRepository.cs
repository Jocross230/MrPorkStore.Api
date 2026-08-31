using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Repositories.Interfaces;

public interface IPigSubmissionRepository
{
    Task<Guid> CreateAsync(PigSubmission submission);

    Task<IEnumerable<PigSubmission>> GetAllAsync();

    Task<PigSubmission?> GetByIdAsync(Guid id);

    Task<bool> UpdateStatusAsync(Guid id, string status);

    Task<bool> DeleteAsync(Guid id);
}