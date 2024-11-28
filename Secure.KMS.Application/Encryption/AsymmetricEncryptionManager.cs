using System;
using EQS.KMS.Application.Enums;

namespace EQS.KMS.Application.Encryption
{
    public static class AsymmetricEncryptionManager
    {
        public static AsymmetricAlgorithmType GetSystemAlgorithmType()
        {
            return AsymmetricAlgorithmType.RSA2048v2;
        }

        public static AsymmetricAlgorithmType GetStrongestEncryptionAlgorithm() // Can be changed without need for data re-encryption.
        {
            return AsymmetricAlgorithmType.RSA2048v2;
        }

        public static AsymmetricEncryptionKeyPair GenerateKey(AsymmetricAlgorithmType asymmetricAlgorithmType, int? keySize = null)
        {
            switch (asymmetricAlgorithmType)
            {
                case AsymmetricAlgorithmType.RSA1024:
                    return EncryptionLogic.GenerateAsymmetricEncryptionKeys(EncryptionKeyLengthType._1024);
                case AsymmetricAlgorithmType.RSA2048v1:
                    return EncryptionLogic.GenerateAsymmetricEncryptionKeys(EncryptionKeyLengthType._2048);
                case AsymmetricAlgorithmType.RSA2048v2:
                    return AsymmetricEncryptionLogicRSA2048v2.GenerateKeys(keySize);
                default:
                    throw new NotImplementedException();
            }
        }

        public static string Encrypt(string plainText, string publicKey, AsymmetricAlgorithmType asymmetricAlgorithmType)
        {
            switch (asymmetricAlgorithmType)
            {
                case AsymmetricAlgorithmType.RSA1024:
                    return AsymmetricEncryptionLogicRSAv1.Encrypt((object)plainText, publicKey, EncryptionKeyLengthType._1024);
                case AsymmetricAlgorithmType.RSA2048v1:
                    return AsymmetricEncryptionLogicRSAv1.Encrypt((object)plainText, publicKey, EncryptionKeyLengthType._2048);
                case AsymmetricAlgorithmType.RSA2048v2:
                    return AsymmetricEncryptionLogicRSA2048v2.Encrypt(plainText, publicKey);
                default:
                    throw new NotImplementedException();
            }
        }

        public static string Decrypt(string encryptedText, string privateKey, AsymmetricAlgorithmType asymmetricAlgorithmType)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return null;

            switch (asymmetricAlgorithmType)
            {
                case AsymmetricAlgorithmType.RSA1024:
                    return AsymmetricEncryptionLogicRSAv1.DecryptObject(encryptedText, privateKey, EncryptionKeyLengthType._1024) + "";
                case AsymmetricAlgorithmType.RSA2048v1:
                    return AsymmetricEncryptionLogicRSAv1.DecryptObject(encryptedText, privateKey, EncryptionKeyLengthType._2048) + "";
                case AsymmetricAlgorithmType.RSA2048v2:
                    return AsymmetricEncryptionLogicRSA2048v2.Decrypt(encryptedText, privateKey);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
