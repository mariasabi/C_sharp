using Order_management.DTOs;
namespace Order_management.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> Register(RegisterRequest request);
        Task<string> Login(LoginRequest request);
        Task<string> ResetPassword(ResetRequest request);
    }
}
