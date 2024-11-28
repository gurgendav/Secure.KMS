using System.Text.Json.Serialization;

namespace EQS.KMS.Application.Models
{
    public class RewrapPayload
    {
        [JsonPropertyName("ciphertext")]
        public string Ciphertext { get; set; }
    }
}