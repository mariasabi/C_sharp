
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Exceptions;
using UserService.Interfaces;
using UserService.Models;

namespace UserService.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

      
        [HttpGet("getUsers")]
        public async Task<ActionResult<User>> GetUsers()
        {
            var response = await _adminService.GetUsers();
            if (response.Count == 0)
            {
                return NotFound("No user found");
            }
            return Ok(response);

        }
        [HttpGet("getUserById")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var response = await _adminService.GetUserById(id);
            if (response == null)
            {
                return NotFound("No user found");
            }
            return Ok(response);
        }
        [HttpGet("getUserByUsername")]
        public async Task<ActionResult<User>> GetUserByUsername(string name)
        {
            var response = await _adminService.GetUserByUsername(name);
            if (response == null)
            {
                return NotFound("No user found");
            }
            return Ok(response);
        }
        [HttpDelete("deleteUser")]
        public async Task<ActionResult<User>> DeleteUserByUsername(string name)
        {
            try
            {
                var username = User.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
                var response = await _adminService.DeleteUserByUsername(name,username);
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex) 
            {
                return Unauthorized(ex.Message);
            }
        }


    }
}
