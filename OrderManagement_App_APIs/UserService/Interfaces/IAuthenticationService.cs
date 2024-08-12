
using UserService.Models;
using UserService.DTOs;

namespace UserService.Interfaces
{
    public interface IAuthenticationService
    {
       
            Task<User> Register(Register request);
            Task<User> Login(Login request);
            Task<string> ResetPassword(Reset request);
         
            string generateJwtToken(User user);
        // string GenerateJSONWebToken(User userInfo);

        Task<string> GetHindiName(string name);

    }
}
