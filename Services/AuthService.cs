using Microsoft.IdentityModel.Tokens;
using MrPorkStore.Api.DTOs.Auth;
using MrPorkStore.Api.Helpers;
using MrPorkStore.Api.Models;
using MrPorkStore.Api.Repositories.Interfaces;
using MrPorkStore.Api.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MrPorkStore.Api.Services;

public class AuthService : IAuthService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthService(
        IAdminRepository adminRepository,
        IConfiguration configuration, IEmailService emailService)
    {
        _adminRepository = adminRepository;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var admin = await _adminRepository.GetByEmailAsync(request.Email);

        if (admin is null)
            return null;

        var passwordIsValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            admin.PasswordHash);

        if (!passwordIsValid)
            return null;

        return GenerateJwtToken(admin);
    }

    public async Task<bool> CreateInitialAdminAsync(
        string email,
        string password)
    {
        var existingAdmin =
            await _adminRepository.GetByEmailAsync(email);

        if (existingAdmin is not null)
            return false;

        var admin = new Admin
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        await _adminRepository.CreateAsync(admin);

        return true;
    }

    public async Task<bool> ChangePasswordAsync(
        Guid adminId,
        ChangePasswordRequest request)
    {
        var admin = await _adminRepository.GetByIdAsync(adminId);

        if (admin is null)
            return false;

        var passwordIsValid = BCrypt.Net.BCrypt.Verify(
            request.CurrentPassword,
            admin.PasswordHash);

        if (!passwordIsValid)
            return false;

        var newPasswordHash =
            BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _adminRepository.UpdatePasswordAsync(
            adminId,
            newPasswordHash);

        return true;
    }
    public async Task ForgotPasswordAsync(
    ForgotPasswordRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var admin = await _adminRepository.GetByEmailAsync(email);

        // Do not reveal whether an email exists.
        if (admin is null)
            return;

        var tokenBytes = RandomNumberGenerator.GetBytes(32);

        var rawToken = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        var tokenHash = TokenHelper.HashToken(rawToken);

        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            AdminId = admin.Id,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        await _adminRepository.CreatePasswordResetTokenAsync(resetToken);

        var resetLink =
    $"http://localhost:8443/admin/reset-password?token={Uri.EscapeDataString(rawToken)}";

        await _emailService.SendPasswordResetEmailAsync(
            admin.Email,
            resetLink);
    }

    public async Task<bool> ResetPasswordAsync(
    ResetPasswordRequest request)
    {
        var tokenHash = TokenHelper.HashToken(request.Token);

        var resetToken =
            await _adminRepository.GetValidPasswordResetTokenAsync(
                tokenHash);

        if (resetToken is null)
            return false;

        var newPasswordHash =
            BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        await _adminRepository.UpdatePasswordAsync(
            resetToken.AdminId,
            newPasswordHash);

        await _adminRepository.MarkPasswordResetTokenAsUsedAsync(
            resetToken.Id);

        return true;
    }

    private LoginResponse GenerateJwtToken(Admin admin)
    {
        var jwtSection = _configuration.GetSection("Jwt");

        var key = jwtSection["Key"]
            ?? throw new InvalidOperationException(
                "JWT Key is not configured.");

        var issuer = jwtSection["Issuer"]
            ?? throw new InvalidOperationException(
                "JWT Issuer is not configured.");

        var audience = jwtSection["Audience"]
            ?? throw new InvalidOperationException(
                "JWT Audience is not configured.");

        var expiresInMinutes =
            int.Parse(jwtSection["ExpiresInMinutes"] ?? "120");

        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                admin.Id.ToString()),

            new Claim(
                JwtRegisteredClaimNames.Email,
                admin.Email),

            new Claim(
                ClaimTypes.NameIdentifier,
                admin.Id.ToString())
        };

        var expiresAt = DateTime.UtcNow.AddMinutes(
            expiresInMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(token),

            ExpiresAt = expiresAt,
            Email = admin.Email
        };
    }
}