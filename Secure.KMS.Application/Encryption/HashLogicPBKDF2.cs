using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EQS.KMS.Application.Encryption
{
    public static class HashLogicPBKDF2
    {
        private static int hashIterations = 1000;

        public static string StretchPasswordToEncryptionKey(string password, string salt = "{E20EC73C-D1A5-4F76-B971-EDAC82E7DD37}")
        {
            // PBKDF2/Rfc2898DeriveBytes
            // http://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129 

            var saltBytes = Encoding.UTF7.GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, hashIterations * 30);
            var hash = pbkdf2.GetBytes(20);

            return Convert.ToBase64String(hash);
        }

        public static PasswordHash GeneratePasswordHash(string password)
        {
            // PBKDF2/Rfc2898DeriveBytes
            // http://stackoverflow.com/questions/4181198/how-to-hash-a-password/10402129#10402129 

            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, hashIterations);
            var hash = pbkdf2.GetBytes(20);

            return new PasswordHash()
            {
                Hash = Convert.ToBase64String(hash),
                Salt = Convert.ToBase64String(salt)
            };
        }

        public static bool VerifyPasswordHash(string password, string passwordHash, string salt)
        {
            var hashBytes = Convert.FromBase64String(passwordHash);
            var saltBytes = Convert.FromBase64String(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, hashIterations);
            var hash = pbkdf2.GetBytes(20);

            return hash.SequenceEqual(hashBytes);
        }
    }
}
