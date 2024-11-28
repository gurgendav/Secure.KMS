using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EQS.KMS.Application.Models
{
    public class LoginPayload
    {
        [JsonPropertyName("customer_id")]
        [Required]
        public string CustomerId { get; set; }

        [JsonPropertyName("user_id")]
        [Required]
        public string UserId { get; set; }

        [JsonPropertyName("password")]
        [Required]
        public string Password { get; set; }
    }
}
