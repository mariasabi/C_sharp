using System.ComponentModel.DataAnnotations;
namespace Order_management.DTOs
{
    public class RegisterRequest
    {
       [Required]
        public string? Username { get; set; }
        [Required]
        public string? Email { get; set; }
       [Required]
        public string? Password { get; set; }
    }
}
