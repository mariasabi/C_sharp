using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.DTOs;
using UserService.Interfaces;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public UserController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]

        public async Task<IActionResult> Login(Login request)
        {

            var user = await _authenticationService.Login(request);
            //   var tokenString = _authenticationService.GenerateJSONWebToken(user);
            var tokenString = _authenticationService.generateJwtToken(user);
            return Ok(tokenString);

        }

        [AllowAnonymous]
        [HttpPost("register")]

        public async Task<IActionResult> Register(Register request)
        {
            var response = await _authenticationService.Register(request);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPut("resetPassword")]

        public async Task<IActionResult> ResetPassword(Reset request)
        {
            var response = await _authenticationService.ResetPassword(request);

            return Ok(response);
        }

        [HttpGet("getUsers")]
        public async Task<ActionResult<User>> GetUsers()
        {
            var response = await _authenticationService.GetUsers();
            if (response.Count == 0)
            {
                return NotFound("No user found");
            }
            return Ok(response);

        }
        [HttpGet("getUser")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var response = await _authenticationService.GetUserById(id);
            if (response== null)
            {
            return NotFound("No user found");
            }
            return Ok(response);
        }
    }
}
