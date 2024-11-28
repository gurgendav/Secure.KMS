using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EQS.KMS.Application.Models
{
    public class CustomerPayload
    {
        [Required]
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [Required]
        [JsonPropertyName("user_password")]
        public string Password { get; set; }
        [Required]
        [JsonPropertyName("customer_name")]
        public string CustomerName { get; set; }
    }

    public class CustomerCreatePayload
    {
        [Required]
        [JsonPropertyName("customer_name")]
        public string CustomerName { get; set; }
    }
}
