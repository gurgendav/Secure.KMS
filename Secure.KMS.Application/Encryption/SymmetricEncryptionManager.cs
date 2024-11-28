using System;
using EQS.KMS.Application.Enums;

namespace EQS.KMS.Application.Encryption
{
    public static class SymmetricEncryptionManager
    {
        public static SymmetricAlgorithmType GetMasterKeyAlgorithmType() // The algo that all keySet's symmetric and private keys are encrypted with.
        {
            return SymmetricAlgorithmType.AES256;
        }

        public static SymmetricAlgorithmType GetSystemAlgorithmType()
        {
            return SymmetricAlgorithmType.AES256;
        }

        public static SymmetricAlgorithmType GetStrongestEncryptionAlgorithm() // Can be changed without need for data re-encryption.
        {
            return SymmetricAlgorithmType.AES256;
        }

        public static SymmetricAlgorithmType GetAlgorithmForKeySetEncryption() // Can only be changed if all KeySets are re-encrypted.
        {
            return SymmetricAlgorithmType.AES256;
        }

        public static string GenerateKey(SymmetricAlgorithmType symmetricAlgorithmType)
        {
            switch (symmetricAlgorithmType)
            {
                case SymmetricAlgorithmType.TripleDES:
                    return SymmetricEncryptionLogicTripleDES.GenerateKey();
                case SymmetricAlgorithmType.AES256:
                    return SymmetricEncryptionLogicAES256.GenerateKey();
                default:
                    throw new NotImplementedException();
            }
        }

        public static string Encrypt(string plainText, string symmetricKey, SymmetricAlgorithmType symmetricAlgorithmType)
        {
            switch (symmetricAlgorithmType)
            {
                case SymmetricAlgorithmType.TripleDES:
                    return SymmetricEncryptionLogicTripleDES.Encrypt((object)plainText, symmetricKey);
                case SymmetricAlgorithmType.AES256:
                    return SymmetricEncryptionLogicAES256.Encrypt(plainText, symmetricKey);
                default:
                    throw new NotImplementedException();
            }

        }


        public static string Decrypt(string encryptedText, string symmetricKey, SymmetricAlgorithmType symmetricAlgorithmType)
        {
            if (string.IsNullOrEmpty(encryptedText))
                return null;

            switch (symmetricAlgorithmType)
            {
                case SymmetricAlgorithmType.TripleDES:
                    return SymmetricEncryptionLogicTripleDES.DecryptObject(encryptedText, symmetricKey) + "";
                case SymmetricAlgorithmType.AES256:
                    return SymmetricEncryptionLogicAES256.Decrypt(encryptedText, symmetricKey);
                default:
                    throw new NotImplementedException();
            }
        }


    }
}
