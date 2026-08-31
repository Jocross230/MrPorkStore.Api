namespace MrPorkStore.Api.Services.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(
        string recipientEmail,
        string resetLink);
}