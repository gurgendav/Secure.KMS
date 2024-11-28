using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using EQS.KMS.Application.Encryption;
using EQS.KMS.Application.Entities;
using EQS.KMS.Application.Enums;
using EQS.KMS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EQS.KMS.Application.Services
{
    public class CryptoService : ICryptoService
    {
        private const int KeySize = 2048;

        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICryptoTokenAccessor _cryptoTokenAccessor;

        public CryptoService(IApplicationDbContext applicationDbContext, ICryptoTokenAccessor cryptoTokenAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _cryptoTokenAccessor = cryptoTokenAccessor;
        }

        private string ComposeCiphertext(AlgorithmType algorithmType, long version, string ciphertext)
        {
            return $"{algorithmType.ToString().ToLowerInvariant()}:v{version}:{ciphertext}";
        }

        private (AlgorithmType algorithmType, long version, string ciphertext) DecomposeCiphertext(string ciphertext)
        {
            var parts = ciphertext.Split(':');
            return (Enum.Parse<AlgorithmType>(parts[0], true), long.Parse(parts[1][1..]), parts[2]);
        }

        private async Task<KeySet> GetLatestKeySet(string customerId)
        {
            return await _applicationDbContext.KeySets.OrderBy(k => k.CreatedDate)
                .LastOrDefaultAsync(k => k.CustomerId == customerId);
        }

        private async Task<KeySet> UnlockKeySet(KeySet keySet)
        {
            var loginToken = _cryptoTokenAccessor.GetToken();

            var session =
                await _applicationDbContext.CryptoSessions.FirstAsync(s => s.Token == loginToken.SessionToken);
            var masterKey = SymmetricEncryptionManager.Decrypt(session.EncryptedMasterKey, loginToken.AuthKey, SymmetricAlgorithmType.AES256);

            var privateKey = SymmetricEncryptionManager.Decrypt(keySet.PrivateKey, masterKey, SymmetricAlgorithmType.AES256);
            var symmetricKey = SymmetricEncryptionManager.Decrypt(keySet.SymmetricKey, masterKey, SymmetricAlgorithmType.AES256);

            return new KeySet
            {
                PrivateKey = privateKey,
                SymmetricKey = symmetricKey,
                CreatedDate = keySet.CreatedDate,
                AsymmetricAlgorithmType = keySet.AsymmetricAlgorithmType,
                HashAlgorithmType = keySet.HashAlgorithmType,
                SymmetricAlgorithmType = keySet.SymmetricAlgorithmType,
                Customer = keySet.Customer,
                CustomerId = keySet.CustomerId,
                Id = keySet.Id,
                PublicKey = keySet.PublicKey
            };
        }

        public async Task<string> EncryptEnvelopeRsa(string customerId, string plainText)
        {
            var keySet = await GetLatestKeySet(customerId);

            var ciphertext = AsymmetricEncryptionManager.Encrypt(plainText, keySet.PublicKey, AsymmetricAlgorithmType.RSA2048v2);

            return ComposeCiphertext(AlgorithmType.Envelope, keySet.Id, ciphertext);
        }

        public async Task<string> EncryptAes(string customerId, string plainText)
        {
            var keySet = await GetLatestKeySet(customerId);

            var newSet = await UnlockKeySet(keySet);

            var ciphertext = SymmetricEncryptionManager.Encrypt(plainText, newSet.SymmetricKey, SymmetricAlgorithmType.AES256);

            return ComposeCiphertext(AlgorithmType.Aes, keySet.Id, ciphertext);
        }


        public async Task<string> EncryptRsa(string customerId, string plainText)
        {
            var keySet = await GetLatestKeySet(customerId);

            var rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize: KeySize);
            rsaCryptoServiceProvider.FromXmlString(keySet.PublicKey);

            byte[] ciphertext = rsaCryptoServiceProvider.Encrypt(Encoding.UTF8.GetBytes(plainText), true);

            return ComposeCiphertext(AlgorithmType.Rsa, keySet.Id, Convert.ToBase64String(ciphertext));
        }

        public async Task<string> Rewrap(string customerId, string ciphertext)
        {
            var parts = DecomposeCiphertext(ciphertext);

            var plaintext = await Decrypt(customerId, ciphertext);

            switch (parts.algorithmType)
            {
                case AlgorithmType.Aes:
                    return await EncryptAes(customerId, plaintext);
                case AlgorithmType.Rsa:
                    return await EncryptRsa(customerId, plaintext);
                case AlgorithmType.Envelope:
                    return await EncryptEnvelopeRsa(customerId, plaintext);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task<string> Decrypt(string customerId, string cipherText)
        {
            var parts = DecomposeCiphertext(cipherText);
            var keySet = await _applicationDbContext.KeySets.FirstAsync(k => k.Id == parts.version && k.CustomerId == customerId);
            

           var newKeySet =  await UnlockKeySet(keySet);

            switch (parts.algorithmType)
            {
                case AlgorithmType.Rsa:
                    return DecryptRsa(newKeySet.PrivateKey, parts.ciphertext);
                case AlgorithmType.Envelope:
                    return AsymmetricEncryptionManager.Decrypt(parts.ciphertext, newKeySet.PrivateKey, AsymmetricAlgorithmType.RSA2048v2);
                case AlgorithmType.Aes:
                    return SymmetricEncryptionManager.Decrypt(parts.ciphertext, newKeySet.SymmetricKey, SymmetricAlgorithmType.AES256);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string DecryptRsa(string privateKey, string ciphertext)
        {
            var rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize: KeySize);
            rsaCryptoServiceProvider.FromXmlString(privateKey);

            var plaintext =
                Encoding.UTF8.GetString(rsaCryptoServiceProvider.Decrypt(Convert.FromBase64String(ciphertext), true));
            return plaintext;
        }
    }
}