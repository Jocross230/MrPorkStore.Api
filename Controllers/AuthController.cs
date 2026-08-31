using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MrPorkStore.Api.DTOs.Auth;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response is null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        return Ok(response);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request)
    {
        var adminIdValue = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(adminIdValue, out var adminId))
        {
            return Unauthorized(new
            {
                message = "Invalid authentication token."
            });
        }

        var changed = await _authService.ChangePasswordAsync(
            adminId,
            request);

        if (!changed)
        {
            return BadRequest(new
            {
                message = "Current password is incorrect."
            });
        }

        return Ok(new
        {
            message = "Password changed successfully."
        });
    }
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
    ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(request);

        // Always return the same response.
        // Do not reveal whether the email exists.
        return Ok(new
        {
            message = "If an account exists with this email, a password reset link has been sent."
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordRequest request)
    {
        var reset = await _authService.ResetPasswordAsync(request);

        if (!reset)
        {
            return BadRequest(new
            {
                message = "The password reset link is invalid, expired, or has already been used."
            });
        }

        return Ok(new
        {
            message = "Password reset successfully."
        });
    }
    [HttpPost("setup-initial-admin")]
    public async Task<IActionResult> SetupInitialAdmin(
    [FromBody] SetupInitialAdminRequest request)
    {
        var created = await _authService.CreateInitialAdminAsync(
            request.Email,
            request.Password);

        if (!created)
        {
            return BadRequest(new
            {
                message = "An admin with this email already exists."
            });
        }

        return Ok(new
        {
            message = "Initial admin created successfully."
        });
    }
}