
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order_management.Interfaces;
using Order_management.Models;
using Order_management.DTOs;


namespace Order_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController :ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        
        public UserController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
       
        public async Task<IActionResult> Login( LoginRequest request)
        {
            var response = await _authenticationService.Login(request);

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
    
        public async Task<ActionResult<User>> Register( RegisterRequest request)
        {
            var response = await _authenticationService.Register(request);
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPut("resetPassword")]
      
        public async Task<IActionResult> ResetPassword(ResetRequest request)
        {
            var response = await _authenticationService.ResetPassword(request);

            return Ok(response);
        }
        
        [HttpGet("getUsers")]
        public async Task<ActionResult<User>> GetUsers()
        {
            var response = await _authenticationService.GetUsers();
            if(response.Count==0)
            {
                return NotFound("No user found");
            }
            return Ok(response);
        }
    }
}
