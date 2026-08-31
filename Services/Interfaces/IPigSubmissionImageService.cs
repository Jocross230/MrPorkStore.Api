using Microsoft.AspNetCore.Http;
using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Services.Interfaces;

public interface IPigSubmissionImageService
{
    Task<PigSubmissionImage> UploadAsync(
        Guid submissionId,
        IFormFile file);

    Task<IEnumerable<PigSubmissionImage>> GetBySubmissionIdAsync(
        Guid submissionId);

    Task<bool> DeleteAsync(Guid imageId);
}