using log4net;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Services
{
    public class AdminService:IAdminService
    {
        private readonly OrderContext _context;
        private readonly IConfiguration _config;
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthService));
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminService(OrderContext context, IConfiguration config, IHttpContextAccessor http)        { 
            _context = context;
            _config = config;
            _httpContextAccessor = http;
        }
        public async Task<ShortUser> GetUserById(int id)
        {
            var user= _context.Users.FirstOrDefault(x => x.Id == id);
            if (user == null) return null;
            if (user.Deleted == false) 
            {
                var shortUser = new ShortUser
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                };
                return shortUser;
            }
            else
                return null;

        }
        public async Task<ShortUser> GetUserByUsername(string name)
        {
            var user=_context.Users.FirstOrDefault(_x => _x.Username == name);
            if (user == null) return null;
            if (user.Deleted == false) 
            {
                var shortUser = new ShortUser
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                };
                return shortUser;
            }
            else
                return null;
        }
        /// <summary>
        /// Retrieve all users.
        /// </summary>
        /// <returns>Task<List<User>></returns>
        public async Task<List<ShortUser>> GetUsers()
        {
            var user = await _context.Users.ToListAsync();

            if (user.Count == 0)
            {
                log.Debug("No users found to retrieve.");
                return null;
            }
            var shortUsers = user
             .Where(x => x.Deleted == false)
              .Select(x => new ShortUser
                {
                    Id = x.Id,
                    Username=x.Username,
                    Email=x.Email,
                    Role=x.Role
                })
              .ToList();

            return shortUsers;
        }
        public async Task<User> DeleteUserByUsername(string name,string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == name);
  
            if (user == null || user.Deleted==true)
            {
                log.Debug("Item cannot be retrieved to delete.");
                throw new ArgumentsException("No such item exists to delete");
            }
            if(user.Role=="Admin")
            {
                log.Debug("User cannot be deleted as role is Admin.");
                throw new UnauthorizedAccessException("User with Admin role cannot be deleted.");

            }

            log.Info($"User with username {user} deleted.");
            //  _context.Users.Remove(user);
            user.Deleted = true;
            user.DeletedBy = username;
            user.DeletedAt= DateTime.Now;
            _context.SaveChanges();
            return user;
        }
    }
}
