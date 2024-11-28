using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EQS.KMS.Application.Models
{
    public class AuthPayload
    {
        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }
    }
}
