using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MrPorkStore.Api.DTOs.ProductImages;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Services;

public class ProductImageService : IProductImageService
{
    private readonly Cloudinary _cloudinary;
    private readonly IProductImageRepository _imageRepository;
    private readonly IProductRepository _productRepository;

    public ProductImageService(
        IConfiguration configuration,
        IProductImageRepository imageRepository,
        IProductRepository productRepository)
    {
        _imageRepository = imageRepository;
        _productRepository = productRepository;

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

    public async Task<ProductImageUploadResult> UploadAsync(
        Guid productId,
        IFormFile file)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null)
        {
            throw new KeyNotFoundException("Product not found.");
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

        if (!allowedTypes.Contains(file.ContentType.ToLower()))
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

            Folder = $"mrporkstore/products/{productId}"
        };

        var uploadResult =
            await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error is not null)
        {
            throw new InvalidOperationException(
                $"Image upload failed: {uploadResult.Error.Message}");
        }

        var existingImages =
            await _imageRepository.GetByProductIdAsync(productId);

        var isPrimary = !existingImages.Any();

        var image = new ProductImage
        {
            ProductId = productId,
            ImageUrl = uploadResult.SecureUrl.ToString(),
            PublicId = uploadResult.PublicId,
            DisplayOrder = existingImages.Count(),
            IsPrimary = isPrimary
        };

        var imageId =
            await _imageRepository.CreateAsync(image);

        return new ProductImageUploadResult
        {
            Id = imageId,
            ProductId = productId,
            ImageUrl = image.ImageUrl,
            PublicId = image.PublicId,
            DisplayOrder = image.DisplayOrder,
            IsPrimary = image.IsPrimary
        };
    }

    public async Task<bool> DeleteAsync(Guid imageId)
    {
        var image = await _imageRepository.GetByIdAsync(imageId);

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
                await _cloudinary.DestroyAsync(deletionParams);

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