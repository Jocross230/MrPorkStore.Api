using System.Security.Cryptography;
using System.Text;

namespace MrPorkStore.Api.Helpers;

public static class TokenHelper
{
    public static string HashToken(string token)
    {
        var tokenBytes = Encoding.UTF8.GetBytes(token);

        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}