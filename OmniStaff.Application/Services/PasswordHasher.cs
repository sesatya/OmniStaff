using System.Security.Cryptography;

namespace OmniStaff.Application.Services;

public static class PasswordHasher
{
    // NOTE: Using a constant salt reduces security. This was requested.
    // Base64 for "TestSaltString1234"
    public const string SaltBase64 = "VGVzdFNhbHRTdHJpbmcxMjM0";

    public static string HashPassword(string password)
    {
        const int iterations = 100_000;
        var salt = Convert.FromBase64String(SaltBase64);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        return $"{iterations}.{SaltBase64}.{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string password, string stored)
    {
        if (string.IsNullOrWhiteSpace(stored)) return false;
        var parts = stored.Split('.');
        if (parts.Length != 3) return false;

        if (!int.TryParse(parts[0], out var iterations)) return false;
        var saltB64 = parts[1];
        var hashB64 = parts[2];

        byte[] salt;
        byte[] expectedHash;
        try
        {
            salt = Convert.FromBase64String(saltB64);
            expectedHash = Convert.FromBase64String(hashB64);
        }
        catch
        {
            return false;
        }

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        var actualHash = pbkdf2.GetBytes(expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
