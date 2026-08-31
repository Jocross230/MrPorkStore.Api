using MrPorkStore.Api.DTOs.PigSubmissions;

namespace MrPorkStore.Api.Services.Interfaces;

public interface IPigSubmissionService
{
    Task<PigSubmissionResponse> CreateAsync(
        CreatePigSubmissionRequest request);

    Task<IEnumerable<PigSubmissionResponse>> GetAllAsync();

    Task<PigSubmissionResponse?> GetByIdAsync(Guid id);

    Task<bool> UpdateStatusAsync(
        Guid id,
        UpdatePigSubmissionStatusRequest request);

    Task<bool> DeleteAsync(Guid id);
}