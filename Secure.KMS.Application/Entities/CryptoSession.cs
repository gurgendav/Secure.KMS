using System;

namespace EQS.KMS.Application.Entities
{
    public class CryptoSession
    {
        public long Id { get; set; }
        
        public string UserId { get; set; }
        public User User { get; set; }
        
        public string EncryptedMasterKey { get; set; }
        public string Token { get; set; }
        
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        
        public long KeySetId { get; set; }
        public KeySet KeySet { get; set; }
        
    }
}