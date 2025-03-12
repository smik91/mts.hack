using System.Security.Cryptography;

namespace Midiot.Common.Helpers;

public static class InformationHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 10000;

    public static string HashText(string text)
    {
        byte[] salt;
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt = new byte[SaltSize]);
        }

        var hash = new Rfc2898DeriveBytes(text, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] hashBytes = hash.GetBytes(HashSize);

        byte[] hashWithSaltBytes = new byte[SaltSize + HashSize];
        Array.Copy(salt, 0, hashWithSaltBytes, 0, SaltSize);
        Array.Copy(hashBytes, 0, hashWithSaltBytes, SaltSize, HashSize);

        return Convert.ToBase64String(hashWithSaltBytes);
    }

    public static bool VerifyText(string text, string storedHash)
    {
        byte[] hashWithSaltBytes = Convert.FromBase64String(storedHash);

        byte[] salt = new byte[SaltSize];
        Array.Copy(hashWithSaltBytes, 0, salt, 0, SaltSize);

        var hash = new Rfc2898DeriveBytes(text, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] hashBytes = hash.GetBytes(HashSize);

        for (int i = 0; i < HashSize; i++)
        {
            if (hashWithSaltBytes[i + SaltSize] != hashBytes[i])
            {
                return false;
            }
        }

        return true;
    }
}
