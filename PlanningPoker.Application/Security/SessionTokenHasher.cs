using System.Security.Cryptography;
using System.Text;

namespace PlanningPoker.Application.Security
{
    public static class SessionTokenHasher
    {
        public static string Hash(string token)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(token);

            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));

            return Convert.ToHexString(bytes);
        }
    }
}
