using System.Threading.Tasks;

namespace EQS.KMS.Application.Interfaces
{
    public interface ICryptoService
    {
        Task<string> EncryptEnvelopeRsa(string customerId, string plainText);
        Task<string> EncryptAes(string customerId, string plainText);
        Task<string> Decrypt(string customerId, string cipherText);
        Task<string> EncryptRsa(string customerId, string encryptPlainText);
        Task<string> Rewrap(string customerId, string ciphertext);
    }
}