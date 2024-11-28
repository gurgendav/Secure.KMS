using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EQS.KMS.Application.Models
{
    public class PasswordResetIO
    {
        [JsonPropertyName("old_password")]
        [Required]
        public string OldPassword { get; set; }

        [JsonPropertyName("new_password")]
        [Required]
        public string NewPassword { get; set; }
    }
}
