using MrPorkStore.Api.DTOs.Auth;

namespace MrPorkStore.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);

    Task<bool> CreateInitialAdminAsync(
        string email,
        string password);

    Task<bool> ChangePasswordAsync(
        Guid adminId,
        ChangePasswordRequest request);

    Task ForgotPasswordAsync(ForgotPasswordRequest request);

    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
}