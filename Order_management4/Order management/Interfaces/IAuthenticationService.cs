using Order_management.DTOs;
using Order_management.Models;
namespace Order_management.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User> Register(RegisterRequest request);
        Task<string> Login(LoginRequest request);
        Task<string> ResetPassword(ResetRequest request);
        Task<List<User>> GetUsers();
    }
}
