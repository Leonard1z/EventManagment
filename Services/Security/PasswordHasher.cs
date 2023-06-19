using System.Security.Cryptography;
using System.Text;

namespace Services.Security
{
    public class PasswordHasher
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="password">adding hash to password</param>
        /// <param name="salt">modifies the salt inside the method</param>
        /// <returns>The hashed password as a string</returns>
        public static string HashPassword(string password, out string salt)
        {
            byte[] saltBytes = GenerateSalt();
            salt = Convert.ToBase64String(saltBytes);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = ComputedHash(passwordBytes, saltBytes);

            return Convert.ToBase64String(hashedBytes);
        }

        public static bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = Convert.FromBase64String(hashedPassword);

            byte[] computedHash = ComputedHash(passwordBytes, saltBytes);

            return hashedBytes.Length == computedHash.Length && hashedBytes.AsSpan().SequenceEqual(computedHash);
        }

        /// <summary>
        /// Generates the salt from random numbers and stores on a 16 byte arr
        /// </summary>
        /// <returns>Generated salt</returns>
        private static byte[] GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return saltBytes;
        }

        /// <summary>
        /// Combines the password and salt using SHA256 hashing Algorithm
        /// </summary>
        /// <param name="passwordBytes"></param>
        /// <param name="saltBytes"></param>
        /// <returns>computed Hash </returns>
        private static byte[] ComputedHash(byte[] passwordBytes, byte[] saltBytes)
        {
            byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);

            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(combinedBytes);
            }
        }
    }
}
