using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Threading.Tasks.Dataflow;

namespace TaskBoard.Api.Handler
{
    public class PasswordHashingHandler
    {
        private const int IterationCount = 100000;
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string HashPassword(string password)
        {
            // Salt
            byte[] salt = new byte[SaltSize];
            Rng.GetBytes(salt);

            // Hash
            byte[] derived = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, IterationCount, HashSize);
            byte[] hash = new byte[SaltSize + HashSize];

            // Cook for 5 minutes
            Array.Copy(salt, 0, hash, 0, SaltSize);
            Array.Copy(derived, 0, hash, SaltSize, HashSize);

            // Dress and serve
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string password, string storedBase64)
        {
            byte[] stored = Convert.FromBase64String(storedBase64);

            if (stored.Length != SaltSize + HashSize)
                return false;

            byte[] salt = new byte[SaltSize];
            Array.Copy(stored, 0, salt, 0, SaltSize);

            byte[] derived = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, IterationCount, HashSize);

            byte[] storedHash = new byte[HashSize];
            Array.Copy(stored, SaltSize, storedHash, 0, HashSize);


            // Constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(derived, storedHash);
        }
    }
}
