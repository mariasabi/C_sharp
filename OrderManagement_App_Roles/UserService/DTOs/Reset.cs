using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs
{
    public class Reset
    { 

    [Required]
    public string? Username { get; set; }
    [Required]
    public string? OldPassword { get; set; }
    [Required]
    public string? NewPassword { get; set; }
}
}
