using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace EQS.KMS.Application.Encryption
{
    public class SymmetricEncryptionLogicTripleDES
    {
        private static Encoding FileEncoding = Encoding.UTF7;

        public static string Encrypt(object input, string key)
        {
            return Encrypt(SerializerLogic.SerializeObject(input), key);
        }

        public static string Encrypt(string input, string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Missing Encryption key");

            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(input);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string GenerateKey()
        {
            return Guid.NewGuid().ToString();
        }

        public static object DecryptObject(string input, string key, bool disableCache = false)
        {
            string value = Decrypt(input, key, disableCache: disableCache);

            // Warning warning warning...
            if (IsBase64String(value))
                try
                {
                    return SerializerLogic.Deserialize(value);
                }
                catch (Exception)
                {
                    return value;
                }
            else
                return value;
        }

        public static bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        public static string Decrypt(string input, string key, bool disableCache = false)
        {
            if (string.IsNullOrEmpty(key))
                throw new Exception("Missing Encryption key");

            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(input);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            var returnObject = UTF8Encoding.UTF8.GetString(resultArray);

            return returnObject;
        }

        public static string Decrypt(byte[] input, string key)
        {
            byte[] keyArray;
            byte[] toEncryptArray = input;
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            var returnObject = UTF8Encoding.UTF8.GetString(resultArray);

            return returnObject;
        }




    }
}
