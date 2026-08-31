using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Services;

public class PigSubmissionImageService : IPigSubmissionImageService
{
    private readonly Cloudinary _cloudinary;
    private readonly IPigSubmissionImageRepository _imageRepository;
    private readonly IPigSubmissionRepository _submissionRepository;

    public PigSubmissionImageService(
        IConfiguration configuration,
        IPigSubmissionImageRepository imageRepository,
        IPigSubmissionRepository submissionRepository)
    {
        _imageRepository = imageRepository;
        _submissionRepository = submissionRepository;

        var cloudName = configuration["Cloudinary:CloudName"]
            ?? throw new InvalidOperationException(
                "Cloudinary CloudName is not configured.");

        var apiKey = configuration["Cloudinary:ApiKey"]
            ?? throw new InvalidOperationException(
                "Cloudinary ApiKey is not configured.");

        var apiSecret = configuration["Cloudinary:ApiSecret"]
            ?? throw new InvalidOperationException(
                "Cloudinary ApiSecret is not configured.");

        var account = new Account(
            cloudName,
            apiKey,
            apiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<PigSubmissionImage> UploadAsync(
        Guid submissionId,
        IFormFile file)
    {
        var submission =
            await _submissionRepository.GetByIdAsync(submissionId);

        if (submission is null)
        {
            throw new KeyNotFoundException(
                "Pig submission not found.");
        }

        if (file is null || file.Length == 0)
        {
            throw new ArgumentException(
                "Please select an image to upload.");
        }

        var allowedTypes = new[]
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        if (!allowedTypes.Contains(
                file.ContentType.ToLower()))
        {
            throw new ArgumentException(
                "Only JPG, PNG, and WEBP images are allowed.");
        }

        // 5 MB maximum
        const long maxFileSize = 5 * 1024 * 1024;

        if (file.Length > maxFileSize)
        {
            throw new ArgumentException(
                "Image size cannot be more than 5 MB.");
        }

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(
                file.FileName,
                stream),

            Folder =
                $"mrporkstore/pig-submissions/{submissionId}"
        };

        var uploadResult =
            await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error is not null)
        {
            throw new InvalidOperationException(
                $"Image upload failed: " +
                $"{uploadResult.Error.Message}");
        }

        var image = new PigSubmissionImage
        {
            PigSubmissionId = submissionId,
            ImageUrl = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId
        };
        return await _imageRepository.CreateAsync(image);
    }

    public async Task<IEnumerable<PigSubmissionImage>>
        GetBySubmissionIdAsync(Guid submissionId)
    {
        var submission =
            await _submissionRepository.GetByIdAsync(submissionId);

        if (submission is null)
        {
            throw new KeyNotFoundException(
                "Pig submission not found.");
        }

        return await _imageRepository
            .GetBySubmissionIdAsync(submissionId);
    }

    public async Task<bool> DeleteAsync(Guid imageId)
    {
        var image =
            await _imageRepository.GetByIdAsync(imageId);

        if (image is null)
        {
            return false;
        }

        // Delete from Cloudinary first
        if (!string.IsNullOrWhiteSpace(image.PublicId))
        {
            var deletionParams =
                new DeletionParams(image.PublicId);

            var deletionResult =
                await _cloudinary.DestroyAsync(
                    deletionParams);

            if (deletionResult.Error is not null)
            {
                throw new InvalidOperationException(
                    $"Cloudinary deletion failed: " +
                    $"{deletionResult.Error.Message}");
            }
        }

        // Then delete from Neon
        return await _imageRepository.DeleteAsync(imageId);
    }
}