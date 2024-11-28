using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace EQS.KMS.Application.Encryption
{
    public class AsymmetricEncryptionLogicRSAv1
    {
        public static string Encrypt(object input, string publicKey, EncryptionKeyLengthType encryptionKeyLengthType)
        {
            return Encrypt(SerializerLogic.SerializeObject(input), publicKey, encryptionKeyLengthType);
        }

        public static string Encrypt(string input, string publicKey, EncryptionKeyLengthType encryptionKeyLengthType)
        {
            if (string.IsNullOrEmpty(publicKey))
                throw new Exception("Missing Encryption key");

            int dwKeySize = EncryptionLogic.ConvertEncryptionKeyLength(encryptionKeyLengthType); // EncryptionLogic.GetEncryptionKeyLength();

            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize: dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(publicKey);
            int keySize = dwKeySize / 8;
            byte[] bytes = Encoding.UTF32.GetBytes(input);

            // int maxLength = ( keySize ) - 2 - ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? maxLength : dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0, tempBytes.Length);
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);
                Array.Reverse(encryptedBytes);
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        //public static object DecryptObject(string input, EncryptionKeyLengthType encryptionKeyLengthType)
        //{
        //    return DecryptObject(input, CoreSettingsLogic.GetPrivateDecryptionKey(), encryptionKeyLengthType);
        //}

        public static object DecryptObject(string input, string privateKey, EncryptionKeyLengthType encryptionKeyLengthType)
        {
            string value = Decrypt(input, privateKey, encryptionKeyLengthType);
            return SerializerLogic.Deserialize(value);
        }

        private static Dictionary<string, RSACryptoServiceProvider> _RSACryptoServiceProviders = new Dictionary<string, RSACryptoServiceProvider>();
        private static Dictionary<string, int> _Base64BlockSizes = new Dictionary<string, int>();

        public static string Decrypt(string input, string privateKey, EncryptionKeyLengthType encryptionKeyLengthType)
        {
            if (string.IsNullOrEmpty(privateKey))
                throw new Exception("Missing Private Encryption key");

            if (!_RSACryptoServiceProviders.ContainsKey(privateKey + encryptionKeyLengthType))
            {
                int dwKeySize = EncryptionLogic.ConvertEncryptionKeyLength(encryptionKeyLengthType);
                RSACryptoServiceProvider rsaCryptoServiceProviderX = new RSACryptoServiceProvider(dwKeySize);
                rsaCryptoServiceProviderX.FromXmlString(privateKey);
                int base64BlockSizeX = ((dwKeySize / 8) % 3 != 0) ? (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;

                _RSACryptoServiceProviders.Add(privateKey + encryptionKeyLengthType, rsaCryptoServiceProviderX);
                _Base64BlockSizes.Add(privateKey + encryptionKeyLengthType, base64BlockSizeX);
            }

            int base64BlockSize = _Base64BlockSizes[privateKey + encryptionKeyLengthType];
            int iterations = input.Length / base64BlockSize;
            if (!string.IsNullOrWhiteSpace(input) && iterations == 0)
                throw new Exception($"Assymetric decryption error. It seems like the input ''{input}'' was not encrypted");

            var rsaCryptoServiceProvider = _RSACryptoServiceProviders[privateKey + encryptionKeyLengthType];
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(input.Substring(base64BlockSize * i, base64BlockSize));
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        }
    }
}
