using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EQS.KMS.Application.Encryption
{
    public static class SymmetricEncryptionLogicAES256
    {
        private static byte[] Salt = Encoding.ASCII.GetBytes("66076491-A6FC-4D2A-94BB-57EDC05A35A0");
        private const int KeyLength = 128;

        public static string GenerateKey()
        {
            using (RandomNumberGenerator rand = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[KeyLength]; // key length
                rand.GetBytes(key);
                return Convert.ToBase64String(key);
            }
        }

        public static string Encrypt(string plainText, string symmetricEncryptionKey)
        {
            byte[] encryptedData = EncryptToByteArray(plainText, symmetricEncryptionKey);
            return Convert.ToBase64String(encryptedData);
        }

        internal static byte[] EncryptToByteArray(string plainText, string symmetricEncryptionKey)
        {
            byte[] encryptedData = null;

            RijndaelManaged rijndael = GetSymmetricAlgorithmForWrite(symmetricEncryptionKey);
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.Write(rijndael.IV, 0, rijndael.IV.Length);

                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        encryptedData = memoryStream.ToArray();
                    }
                }
            }

            return encryptedData;
        }


        public static string Decrypt(string encryptedText, string symmetricEncryptionKey)
        {
            string decryptedText = string.Empty;
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            return DecryptFromByteArray(encryptedBytes.ToArray(), symmetricEncryptionKey);
        }

        public static string DecryptFromByteArray(byte[] encryptedBytes, string symmetricEncryptionKey)
        {
            string decryptedText = string.Empty;

            RijndaelManaged rijndael = GetSymmetricAlgorithmForRead(symmetricEncryptionKey);
            {
                rijndael.IV = encryptedBytes.Take(rijndael.KeySize / 16).ToArray();

                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                {
                    memoryStream.Position = rijndael.KeySize / 16;

                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, rijndael.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            decryptedText = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            return decryptedText;
        }


        private static Dictionary<string, RijndaelManaged> _RijndaelManaged = new Dictionary<string, RijndaelManaged>();
        private static object RijndaelManagedLock = "";

        internal static RijndaelManaged GetSymmetricAlgorithmForRead(string symmetricEncryptionKey)
        {
            if (_RijndaelManaged.ContainsKey(symmetricEncryptionKey))
                return _RijndaelManaged[symmetricEncryptionKey];

            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.KeySize = 256; //max

            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(symmetricEncryptionKey, Salt);
            rijndael.Key = key.GetBytes(rijndael.KeySize / 8);

            lock (RijndaelManagedLock)
            {
                if (!_RijndaelManaged.ContainsKey(symmetricEncryptionKey))
                    _RijndaelManaged.Add(symmetricEncryptionKey, rijndael);
            }

            return rijndael;
        }

        internal static RijndaelManaged GetSymmetricAlgorithmForWrite(string symmetricEncryptionKey)
        {
            RijndaelManaged rijndael = new RijndaelManaged();
            rijndael.KeySize = 256; //max

            Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(symmetricEncryptionKey, Salt);
            rijndael.Key = key.GetBytes(rijndael.KeySize / 8);

            return rijndael;
        }
    }
}
