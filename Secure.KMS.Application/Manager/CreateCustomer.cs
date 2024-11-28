using EQS.KMS.Application.Encryption;
using EQS.KMS.Application.Entities;
using EQS.KMS.Application.Interfaces;
using EQS.KMS.Application.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace EQS.KMS.Application.Manager
{
    public class CreateCustomer
    {
        private readonly IApplicationDbContext _dbContext;

        public CreateCustomer(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<HttpStatusCode> CreateFirstCustomerUserKeySet(string customerId, CustomerPayload payload)
        {
            if (await _dbContext.Customers.AnyAsync(a => a.Id == customerId))
            {
                return HttpStatusCode.Conflict;
                // Customer already exists
            }

            if (await _dbContext.Users.AnyAsync(a => a.Id == payload.UserId))
            {
                return HttpStatusCode.Conflict;
                // User already exists
            }

            var customer = new Customer
            {
                Id = customerId,
                Name = payload.CustomerName
            };

            _dbContext.Customers.Add(customer);

            var masterKey =
                SymmetricEncryptionManager.GenerateKey(SymmetricEncryptionManager.GetStrongestEncryptionAlgorithm());
            var asymmetricEncryptionKeyPair =
                AsymmetricEncryptionManager.GenerateKey(AsymmetricEncryptionManager.GetSystemAlgorithmType());
            var symmetricEncryptionKey =
                SymmetricEncryptionManager.GenerateKey(SymmetricEncryptionManager.GetSystemAlgorithmType());

            var passwordHash =
                HashManager.GeneratePasswordHash(payload.Password, HashManager.GetSystemAlgorithmType());

            var keySet = new KeySet
            {
                Id = 1,
                CustomerId = customerId,
                AsymmetricAlgorithmType = AsymmetricEncryptionManager.GetSystemAlgorithmType(),
                HashAlgorithmType = HashManager.GetSystemAlgorithmType(),
                PrivateKey = asymmetricEncryptionKeyPair.PrivateDecryptionKey,
                PublicKey = asymmetricEncryptionKeyPair.PublicEncryptionKey,
                SymmetricKey = symmetricEncryptionKey,
                SymmetricAlgorithmType = SymmetricEncryptionManager.GetMasterKeyAlgorithmType(),
                CreatedDate = DateTime.UtcNow
            };

            keySet.PrivateKey = SymmetricEncryptionManager.Encrypt(keySet.PrivateKey, masterKey,
                SymmetricEncryptionManager.GetAlgorithmForKeySetEncryption());
            keySet.SymmetricKey = SymmetricEncryptionManager.Encrypt(keySet.SymmetricKey, masterKey,
                SymmetricEncryptionManager.GetAlgorithmForKeySetEncryption());

            _dbContext.KeySets.Add(keySet);

            var user = new User
            {
                Id = payload.UserId,
                CustomerId = customerId,
                PasswordHash = passwordHash.Hash,
                PasswordSalt = passwordHash.Salt,
                EncryptedMasterKey = SymmetricEncryptionManager.Encrypt(masterKey,
                    HashManager.StretchPasswordToEncryptionKey(HashManager.GetSystemAlgorithmType(),
                        payload.Password), SymmetricEncryptionManager.GetMasterKeyAlgorithmType()),
                MasterKeyHashAlgorithmType = HashManager.GetSystemAlgorithmType(),
                MasterKeySymmetricAlgorithmType = SymmetricEncryptionManager.GetSystemAlgorithmType()
            };

            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();

            return HttpStatusCode.Created;
        }

        public async Task<Customer> Update(string customerId, CustomerCreatePayload payload)
        {
            var customer = _dbContext.Customers.FirstOrDefault(a => a.Id == customerId);

            // TODO: throw exception if customer does not exist
            if (customer != null)
            {
                customer.Name = payload.CustomerName;
                _dbContext.Customers.Update(customer);

                await _dbContext.SaveChangesAsync();
            }

            return customer;
        }
    }
}