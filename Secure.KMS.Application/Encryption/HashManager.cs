using System;
using EQS.KMS.Application.Enums;

namespace EQS.KMS.Application.Encryption
{
    public static class HashManager
    {
        public static HashAlgorithmType GetSystemAlgorithmType()
        {
            return HashAlgorithmType.PBKDF2;
        }

        public static HashAlgorithmType GetStrongestAlgorithmType() // Can be changed without need for data re-encryption.
        {
            return HashAlgorithmType.PBKDF2;
        }

        public static string GetPublicUserPasswordHash(HashAlgorithmType hashAlgorithmType, string password)
        {
            switch (hashAlgorithmType)
            {
                case HashAlgorithmType.MD5:
                    return HashLogic.GetHash(password);
                case HashAlgorithmType.PBKDF2:
                    return HashLogicPBKDF2.StretchPasswordToEncryptionKey(password);
                default:
                    throw new NotImplementedException();
            }
        }

        public static string StretchPasswordToEncryptionKey(HashAlgorithmType hashAlgorithmType, string password)
        {
            switch (hashAlgorithmType)
            {
                case HashAlgorithmType.MD5:
                    return password;
                case HashAlgorithmType.PBKDF2:
                    return HashLogicPBKDF2.StretchPasswordToEncryptionKey(password);
                default:
                    throw new NotImplementedException();
            }
        }

        public static PasswordHash StretchPasswordToPasswordHash(string password)
        {
            return HashLogicPBKDF2.GeneratePasswordHash(password);
        }

        public static bool VerifyPasswordHash(string password, string passwordHash, string passwordSalt, HashAlgorithmType masterKeyHashAlgorithmType)
        {
            switch (masterKeyHashAlgorithmType)
            {
                case HashAlgorithmType.MD5:
                    string passwordHashWithSalt = HashLogic.GetHashWithSalt(password, passwordSalt);
                    return (passwordHashWithSalt == passwordHash);
                case HashAlgorithmType.PBKDF2:
                    return HashLogicPBKDF2.VerifyPasswordHash(password, passwordHash, passwordSalt);
                default:
                    throw new NotImplementedException();
            }
        }

        public static PasswordHash GeneratePasswordHash(string password, HashAlgorithmType hashAlgorithmType)
        {
            switch (hashAlgorithmType)
            {
                case HashAlgorithmType.MD5:
                    string salt = Guid.NewGuid().ToString();
                    string passwordHash = HashLogic.GetHashWithSalt(password, salt);
                    return new PasswordHash() { Hash = passwordHash, Salt = salt };
                case HashAlgorithmType.PBKDF2:
                    return HashLogicPBKDF2.GeneratePasswordHash(password);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
