using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EQS.KMS.Application.Models
{
    public class KeyPayload
    {
        [JsonPropertyName("key_size")]
        [Required]
        public int KeySize { get; set; }
    }
}
