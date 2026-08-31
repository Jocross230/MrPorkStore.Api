using Resend;
using MrPorkStore.Api.Services.Interfaces;

namespace MrPorkStore.Api.Services;

public class EmailService : IEmailService
{
    private readonly IResend _resend;

    public EmailService(IResend resend)
    {
        _resend = resend;
    }

    public async Task SendPasswordResetEmailAsync(
        string recipientEmail,
        string resetLink)
    {
        var message = new EmailMessage
        {
            From = "Mr.Pork Store <onboarding@resend.dev>",
            Subject = "Reset Your Mr.Pork Store Password",
            HtmlBody = $"""
                <div style="font-family: Arial, sans-serif; max-width: 600px; margin: auto;">
                    <h2 style="color: #a71919;">Mr.Pork Store</h2>

                    <p>Hello,</p>

                    <p>
                        We received a request to reset your Mr.Pork Store
                        administrator password.
                    </p>

                    <p>
                        Click the button below to create a new password:
                    </p>

                    <p>
                        <a href="{resetLink}"
                           style="
                               display: inline-block;
                               padding: 12px 24px;
                               background-color: #a71919;
                               color: white;
                               text-decoration: none;
                               border-radius: 6px;
                               font-weight: bold;
                           ">
                            Reset Password
                        </a>
                    </p>

                    <p>
                        If you did not request a password reset, you can
                        safely ignore this email.
                    </p>

                    <p>
                        This password reset link will expire for security
                        reasons.
                    </p>

                    <p>
                        Regards,<br/>
                        <strong>Mr.Pork Store</strong>
                    </p>
                </div>
                """
        };

        message.To.Add(recipientEmail);

        await _resend.EmailSendAsync(message);
    }
}