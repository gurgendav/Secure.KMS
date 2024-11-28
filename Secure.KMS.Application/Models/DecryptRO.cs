using System.Text.Json.Serialization;

namespace EQS.KMS.Application.Models
{
    public  class DecryptIO
    {
        [JsonPropertyName("ciphertext")]
        public string CipherText { get; set; }
    }

    public class DecryptRO
    {
        [JsonPropertyName("plaintext")]
        public string PlainText { get; set; }
    }
}
