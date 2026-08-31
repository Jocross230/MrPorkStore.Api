using System.ComponentModel.DataAnnotations;

namespace MrPorkStore.Api.DTOs.Auth;

public class SetupInitialAdminRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = string.Empty;
}