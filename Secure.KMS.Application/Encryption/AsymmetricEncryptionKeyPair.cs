namespace EQS.KMS.Application.Encryption
{
    public class AsymmetricEncryptionKeyPair
    {
        public string PublicEncryptionKey { get; set; }
        public string PrivateDecryptionKey { get; set; }

        public AsymmetricEncryptionKeyPair(string publicEncryptionKey, string privateDecryptionKey)
        {
            PublicEncryptionKey = publicEncryptionKey;
            PrivateDecryptionKey = privateDecryptionKey;
        }
    }
}
