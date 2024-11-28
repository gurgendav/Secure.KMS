using EQS.KMS.Application.Enums;

namespace EQS.KMS.Application.Entities
{
    public class User
    {
        public string Id { get; set; }
        
        public string CustomerId { get; set; }
        
        public string PasswordHash { get; set; }
        
        public string PasswordSalt { get; set; }
        
        public string EncryptedMasterKey { get; set; }
        
        public HashAlgorithmType MasterKeyHashAlgorithmType { get; set; }
        public SymmetricAlgorithmType MasterKeySymmetricAlgorithmType { get; set; }
    }
}