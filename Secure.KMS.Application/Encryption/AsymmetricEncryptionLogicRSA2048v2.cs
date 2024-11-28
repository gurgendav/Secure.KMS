using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EQS.KMS.Application.Encryption
{
    internal static class AsymmetricEncryptionLogicRSA2048v2
    {
        private const int KeySize = 2048;

        public static string Encrypt(string plainText, string publicKey)
        {
            // Encrypt data with unique symmetric key. That key is asymmetric encrypted and prepended data:

            string symmetricKey = SymmetricEncryptionLogicAES256.GenerateKey();
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize: KeySize);
            rsaCryptoServiceProvider.FromXmlString(publicKey);

            byte[] encryptedSymmetricKey = rsaCryptoServiceProvider.Encrypt(Encoding.UTF8.GetBytes(symmetricKey), true);
            byte[] encryptedBytes = SymmetricEncryptionLogicAES256.EncryptToByteArray(plainText, symmetricKey);

            return Convert.ToBase64String(encryptedSymmetricKey.Concat(encryptedBytes).ToArray());
        }

        public static string Decrypt(string encryptedText, string privateKey)
        {
            // Encrypt data with unique symmetric key. That key is asymmetric encrypted and prepended data:

            int encryptedSymmetricKeyLength = KeySize / 8;
            byte[] encryptedBytes = new byte[0];
            try
            {
                encryptedBytes = Convert.FromBase64String(encryptedText);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string encryptedSymmetricKey = Convert.ToBase64String(encryptedBytes.Take(encryptedSymmetricKeyLength).ToArray());

            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize: KeySize);
            rsaCryptoServiceProvider.FromXmlString(privateKey);

            string symmetricKey = Encoding.UTF8.GetString(rsaCryptoServiceProvider.Decrypt(Convert.FromBase64String(encryptedSymmetricKey), true));

            string decryptedText = SymmetricEncryptionLogicAES256.DecryptFromByteArray(encryptedBytes.Skip(encryptedSymmetricKeyLength).ToArray(), symmetricKey);

            return decryptedText;
        }

        public static AsymmetricEncryptionKeyPair GenerateKeys(int? keySize = null)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(keySize > 0 ? KeySize : KeySize);

            return new AsymmetricEncryptionKeyPair(rsaProvider.ToXmlString(false), rsaProvider.ToXmlString(true));
        }
    }
}
