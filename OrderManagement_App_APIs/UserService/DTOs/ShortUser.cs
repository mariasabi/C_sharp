namespace UserService.DTOs
{
    public class ShortUser
    {
        public int Id { get; set; } = 0;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}