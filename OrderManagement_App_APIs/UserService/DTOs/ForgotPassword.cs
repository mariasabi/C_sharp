using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
 
 
}
