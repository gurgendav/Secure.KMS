using System.Text.Json.Serialization;

namespace EQS.KMS.Application.Models
{
    public class CreateUserIO
    {
        [JsonPropertyName("password")]
        public string Password { get; set; } 
    }
}