using MrPorkStore.Api.DTOs.PigSubmissions;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Services;

public class PigSubmissionService : IPigSubmissionService
{
    private readonly IPigSubmissionRepository _repository;
    private readonly IPigSubmissionImageRepository _imageRepository;

    private static readonly string[] AllowedStatuses =
    [
        "NEW",
        "CONTACTED",
        "APPROVED",
        "REJECTED"
    ];

    public PigSubmissionService(
        IPigSubmissionRepository repository,
        IPigSubmissionImageRepository imageRepository)
    {
        _repository = repository;
        _imageRepository = imageRepository;
    }

    public async Task<PigSubmissionResponse> CreateAsync(
        CreatePigSubmissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FarmerName))
            throw new ArgumentException("Farmer name is required.");

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            throw new ArgumentException("Phone number is required.");

        if (request.Weight.HasValue && request.Weight <= 0)
            throw new ArgumentException(
                "Weight must be greater than zero.");

        if (request.ExpectedPrice.HasValue &&
            request.ExpectedPrice <= 0)
            throw new ArgumentException(
                "Expected price must be greater than zero.");

        var submission = new PigSubmission
        {
            FarmerName = request.FarmerName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = request.Email?.Trim(),
            Location = request.Location?.Trim(),
            PigDetails = request.PigDetails?.Trim(),
            Weight = request.Weight,
            ExpectedPrice = request.ExpectedPrice,
            Status = "NEW"
        };

        var id = await _repository.CreateAsync(submission);

        var createdSubmission =
            await _repository.GetByIdAsync(id)
            ?? throw new InvalidOperationException(
                "Pig submission was created but could not be retrieved.");

        return await MapToResponseAsync(createdSubmission);
    }

    public async Task<IEnumerable<PigSubmissionResponse>> GetAllAsync()
    {
        var submissions = await _repository.GetAllAsync();

        var responses = new List<PigSubmissionResponse>();

        foreach (var submission in submissions)
        {
            responses.Add(
                await MapToResponseAsync(submission));
        }

        return responses;
    }

    public async Task<PigSubmissionResponse?> GetByIdAsync(Guid id)
    {
        var submission = await _repository.GetByIdAsync(id);

        if (submission is null)
            return null;

        return await MapToResponseAsync(submission);
    }

    public async Task<bool> UpdateStatusAsync(
        Guid id,
        UpdatePigSubmissionStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
            throw new ArgumentException("Status is required.");

        var normalizedStatus = request.Status.Trim();

        var validStatus = AllowedStatuses.Any(status =>
            string.Equals(
                status,
                normalizedStatus,
                StringComparison.OrdinalIgnoreCase));

        if (!validStatus)
        {
            throw new ArgumentException(
                "Invalid status. Allowed values are: " +
                "NEW, CONTACTED, APPROVED, REJECTED.");
        }

        var statusToSave = AllowedStatuses.First(status =>
            string.Equals(
                status,
                normalizedStatus,
                StringComparison.OrdinalIgnoreCase));

        return await _repository.UpdateStatusAsync(
            id,
            statusToSave);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    private async Task<PigSubmissionResponse> MapToResponseAsync(
        PigSubmission submission)
    {
        var images =
            await _imageRepository.GetBySubmissionIdAsync(
                submission.Id);

        return new PigSubmissionResponse
        {
            Id = submission.Id,
            FarmerName = submission.FarmerName,
            PhoneNumber = submission.PhoneNumber,
            Email = submission.Email,
            Location = submission.Location,
            PigDetails = submission.PigDetails,
            Weight = submission.Weight,
            ExpectedPrice = submission.ExpectedPrice,
            Status = submission.Status,
            CreatedAt = submission.CreatedAt,
            UpdatedAt = submission.UpdatedAt,
            Images = images
        };
    }
}