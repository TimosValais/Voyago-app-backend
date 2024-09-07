using System.Security.Cryptography;

namespace Voyago.Auth.BusinessLogic.Helpers;

public static class PasswordHelper
{
    private const int SaltSize = 16; // 128-bit
    private const int KeySize = 32;  // 256-bit
    private const int Iterations = 10000;

    // Delimiter to split the hashed password from the salt
    private static readonly char Delimiter = ':';

    public static string HashPassword(string password)
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[SaltSize];
        rng.GetBytes(salt);

        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize
        );

        // Combine salt and hash into a single string for storage
        return $"{Convert.ToBase64String(salt)}{Delimiter}{Convert.ToBase64String(hash)}";
    }

    public static bool CheckPassword(string password, string storedHash)
    {
        // Split the stored hash into salt and hash
        string[] parts = storedHash.Split(Delimiter);
        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        // Hash the incoming password with the same salt and parameters
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            KeySize
        );

        // Compare the stored hash with the newly generated hash
        return CryptographicOperations.FixedTimeEquals(hash, hashToCompare);
    }
}
