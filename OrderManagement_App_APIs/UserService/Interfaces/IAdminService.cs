using UserService.Models;

namespace UserService.Interfaces
{
    public interface IAdminService
    {
        Task<User> GetUserById(int id);
        Task<List<User>> GetUsers();
         Task<User> DeleteUserByUsername(string name,string username);
        Task<User> GetUserByUsername(string name);
    }
}
