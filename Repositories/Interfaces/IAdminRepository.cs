using MrPorkStore.Api.Models;

namespace MrPorkStore.Api.Repositories.Interfaces;

public interface IAdminRepository
{
    Task<Admin?> GetByEmailAsync(string email);

    Task<Admin?> GetByIdAsync(Guid id);

    Task<Guid> CreateAsync(Admin admin);

    Task UpdatePasswordAsync(Guid adminId, string passwordHash);

    Task CreatePasswordResetTokenAsync(
        PasswordResetToken resetToken);

    Task<PasswordResetToken?> GetValidPasswordResetTokenAsync(
        string tokenHash);

    Task MarkPasswordResetTokenAsUsedAsync(Guid tokenId);
}