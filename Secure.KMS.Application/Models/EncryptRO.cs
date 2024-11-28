using System.Text.Json.Serialization;

namespace EQS.KMS.Application.Models
{
    public  class EncryptRO
    {
        [JsonPropertyName("ciphertext")]
        public string CipherText { get; set; }
    }

    public class EncryptIO
    {
        [JsonPropertyName("plaintext")]
        public string PlainText { get; set; }
    }
}
