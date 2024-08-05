
using UserService.Models;
using UserService.DTOs;

namespace UserService.Interfaces
{
    public interface IAuthenticationService
    {
       
            Task<User> Register(Register request);
            Task<User> Login(Login request);
            Task<string> ResetPassword(Reset request);
            Task<List<User>> GetUsers();
        Task<User> GetUserById(int id);
         
            string generateJwtToken(User user);
            // string GenerateJSONWebToken(User userInfo);

        
   

    }
}
