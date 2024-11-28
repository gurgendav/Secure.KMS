using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EQS.KMS.Application.Models
{
    public class LoginToken
    {
        [JsonPropertyName("session_token")]
        [Required]
        public string SessionToken { get; set; }

        [JsonPropertyName("authentication_key")]
        [Required]
        public string AuthKey { get; set; }
    }
}
