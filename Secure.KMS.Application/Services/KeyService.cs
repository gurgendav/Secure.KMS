using System;
using System.Linq;
using System.Threading.Tasks;
using EQS.KMS.Application.Encryption;
using EQS.KMS.Application.Entities;
using EQS.KMS.Application.Interfaces;
using EQS.KMS.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace EQS.KMS.Application.Services
{
    public class KeyService : IKeyService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ICryptoTokenAccessor _cryptoTokenAccessor;

        public KeyService(IApplicationDbContext dbContext, ICryptoTokenAccessor cryptoTokenAccessor)
        {
            _dbContext = dbContext;
            _cryptoTokenAccessor = cryptoTokenAccessor;
        }
        
        public async Task RotateKeySet(string customerId, KeyPayload payload)
        {
            var token = _cryptoTokenAccessor.GetToken();
            var session = await _dbContext.CryptoSessions.FirstAsync(x => x.Token == token.SessionToken);

            var masterKey = SymmetricEncryptionManager.Decrypt(session.EncryptedMasterKey, token.AuthKey,
                SymmetricEncryptionManager.GetStrongestEncryptionAlgorithm());
            
            var asymmetricEncryptionKeyPair =
                AsymmetricEncryptionManager.GenerateKey(AsymmetricEncryptionManager.GetSystemAlgorithmType(), payload.KeySize);
            var symmetricEncryptionKey =
                SymmetricEncryptionManager.GenerateKey(SymmetricEncryptionManager.GetSystemAlgorithmType());

            var keySet = new KeySet
            {
                Id = _dbContext.KeySets.OrderBy(x => x.CreatedDate).Last().Id + 1,
                AsymmetricAlgorithmType = AsymmetricEncryptionManager.GetSystemAlgorithmType(),
                HashAlgorithmType = HashManager.GetSystemAlgorithmType(),
                PrivateKey = SymmetricEncryptionManager.Encrypt(asymmetricEncryptionKeyPair.PrivateDecryptionKey, masterKey,
                    SymmetricEncryptionManager.GetAlgorithmForKeySetEncryption()),
                PublicKey = asymmetricEncryptionKeyPair.PublicEncryptionKey,
                SymmetricKey = SymmetricEncryptionManager.Encrypt(symmetricEncryptionKey, masterKey,
                    SymmetricEncryptionManager.GetAlgorithmForKeySetEncryption()),
                SymmetricAlgorithmType = SymmetricEncryptionManager.GetMasterKeyAlgorithmType(),
                CreatedDate = DateTime.Now,
                CustomerId = customerId
            };

            _dbContext.KeySets.Add(keySet);

            await _dbContext.SaveChangesAsync();
        }
    }

}