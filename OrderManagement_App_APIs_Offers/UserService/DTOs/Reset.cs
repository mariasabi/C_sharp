using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs
{
    public class Reset
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}