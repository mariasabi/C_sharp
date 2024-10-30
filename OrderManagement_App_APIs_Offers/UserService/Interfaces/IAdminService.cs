using UserService.DTOs;
using UserService.Models;
using UserService.Services;

namespace UserService.Interfaces
{
    public interface IAdminService
    {
        Task<ShortUser> GetUserById(int id);
        Task<List<ShortUser>> GetUsers();
         Task<User> DeleteUserByUsername(string name,string username);
        Task<ShortUser> GetUserByUsername(string name);
    }
}
