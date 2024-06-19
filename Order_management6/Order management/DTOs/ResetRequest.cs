using System.ComponentModel.DataAnnotations;
namespace Order_management.DTOs
{
    public class ResetRequest
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? OldPassword { get; set; }
        [Required]
        public string? NewPassword { get; set; }
    }
}
