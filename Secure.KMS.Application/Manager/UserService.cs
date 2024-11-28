using EQS.KMS.Application.Encryption;
using EQS.KMS.Application.Entities;
using EQS.KMS.Application.Interfaces;
using EQS.KMS.Application.Models;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EQS.KMS.Application.Manager
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbContext _db;
        private readonly ICryptoTokenAccessor _cryptoTokenAccessor;

        public UserService(IApplicationDbContext db, ICryptoTokenAccessor cryptoTokenAccessor)
        {
            _db = db;
            _cryptoTokenAccessor = cryptoTokenAccessor;
        }
        
        public void UpdatePassword(string userId, PasswordResetIO passwordReset)
        {
            var user = _db.Users.Find(userId);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var masterKey = SymmetricEncryptionManager.Decrypt(user.EncryptedMasterKey,
                HashManager.StretchPasswordToEncryptionKey(user.MasterKeyHashAlgorithmType,
                    passwordReset.OldPassword), user.MasterKeySymmetricAlgorithmType);
            user.EncryptedMasterKey = SymmetricEncryptionManager.Encrypt(masterKey,
                HashManager.StretchPasswordToEncryptionKey(user.MasterKeyHashAlgorithmType,
                    passwordReset.NewPassword), user.MasterKeySymmetricAlgorithmType);

            var passwordHash =
                HashManager.GeneratePasswordHash(passwordReset.NewPassword, HashManager.GetSystemAlgorithmType());

            user.PasswordHash = passwordHash.Hash;
            user.PasswordSalt = passwordHash.Salt;
            _db.SaveChangesAsync();
        }

        public async Task CreateNewUser(string customerId, string userId, string password)
        {
            var token = _cryptoTokenAccessor.GetToken();
            var session = await _db.CryptoSessions.FirstAsync(s => s.Token == token.SessionToken);

            var masterKey = SymmetricEncryptionManager.Decrypt(session.EncryptedMasterKey, token.AuthKey,
                SymmetricEncryptionManager.GetMasterKeyAlgorithmType());

            var passwordHash =
                HashManager.GeneratePasswordHash(password, HashManager.GetSystemAlgorithmType());

            var user = new User()
            {
                Id = userId,
                CustomerId = customerId,
                PasswordHash = passwordHash.Hash,
                PasswordSalt = passwordHash.Salt,
                EncryptedMasterKey = SymmetricEncryptionManager.Encrypt(masterKey, HashManager.StretchPasswordToEncryptionKey(HashManager.GetSystemAlgorithmType(), password), SymmetricEncryptionManager.GetMasterKeyAlgorithmType()),
                MasterKeyHashAlgorithmType = HashManager.GetSystemAlgorithmType(),
                MasterKeySymmetricAlgorithmType = SymmetricEncryptionManager.GetSystemAlgorithmType()
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
    }
}