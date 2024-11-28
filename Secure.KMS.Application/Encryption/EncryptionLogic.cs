using System;
using System.Security.Cryptography;

namespace EQS.KMS.Application.Encryption
{
    public class EncryptionLogic
    {
        public static EncryptionKeyLengthType GetNewSiteDefault()
        {
            return EncryptionKeyLengthType._2048;
        }

        public static AsymmetricEncryptionKeyPair GenerateAsymmetricEncryptionKeys(EncryptionKeyLengthType encryptionKeyLengthType)
        {
            int dwKeySize = EncryptionLogic.ConvertEncryptionKeyLength(encryptionKeyLengthType);

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(dwKeySize);
            string privateKey = rsaProvider.ToXmlString(true);
            string publicKey = rsaProvider.ToXmlString(false);

            return new AsymmetricEncryptionKeyPair(publicKey, privateKey);
        }

        public static int ConvertEncryptionKeyLength(EncryptionKeyLengthType encryptionKeyLength)
        {
            int dwKeySize = 0;
            switch (encryptionKeyLength)
            {
                case EncryptionKeyLengthType._1024:
                    dwKeySize = 1024;
                    break;
                case EncryptionKeyLengthType._2048:
                    dwKeySize = 2048;
                    break;
                case EncryptionKeyLengthType.NA:
                default:
                    throw new Exception("Missing Encryption key length");
            }

            return dwKeySize;
        }
    }
}
