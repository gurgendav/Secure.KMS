using System;
using EQS.KMS.Application.Enums;
using HashAlgorithmType = EQS.KMS.Application.Enums.HashAlgorithmType;

namespace EQS.KMS.Application.Entities
{
    public class KeySet
    {
        public long Id { get; set; }
        public string SymmetricKey { get; set; }
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        public SymmetricAlgorithmType SymmetricAlgorithmType { get; set; }
        public AsymmetricAlgorithmType AsymmetricAlgorithmType { get; set; }
        public HashAlgorithmType HashAlgorithmType { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}