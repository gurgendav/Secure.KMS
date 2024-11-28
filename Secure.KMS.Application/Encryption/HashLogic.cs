using System;
using System.Security.Cryptography;
using System.Text;

namespace EQS.KMS.Application.Encryption
{
    public class HashLogic
    {
        public static string GetHash(string input)
        {
            byte[] keyArray;

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(input));
            hashmd5.Clear();

            return Convert.ToBase64String(keyArray, 0, keyArray.Length);
        }

        public static string GetHashWithSalt(string password, string salt)
        {
            return GetHash(password + salt);
        }
    }
}
