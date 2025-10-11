using System.Security.Cryptography;

namespace TaskBoard.Api.Services.Helper
{
    public static class ApiKeyHelper
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        public static string GenerateApiKey(int length = 32)
        {
            byte[] bytes = new byte[length];
            Rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
