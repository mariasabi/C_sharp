using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.DTOs;
using UserService.Interfaces;
using UserService.Exceptions;
using Microsoft.AspNetCore.Components.Forms;

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
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword model)
        {
            try
            {
                var response = await _authenticationService.ForgotPassword(model);
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
                    
            }
        }

        [AllowAnonymous]
        [HttpPut("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] Reset request)
        {
            try
            {
                var response = await _authenticationService.ResetPassword(request);
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("validateOTP")]
        public IActionResult ValidateOTP([FromBody] ForgotPassword model, [FromQuery] string otp)
        {
            try
            {
                var response = _authenticationService.ValidateOTP(model.Email, otp);
               
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]

        public async Task<IActionResult> Login(Login request)
        {
            try
            {
                var user = await _authenticationService.Login(request);
                //   var tokenString = _authenticationService.GenerateJSONWebToken(user);
                var tokenString = _authenticationService.generateJwtToken(user);
                return Ok(tokenString);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", details = ex.Message });
            }

        }

        [AllowAnonymous]
        [HttpPost("register")]

        public async Task<IActionResult> Register(Register request)
        {
            try
            {
                var response = await _authenticationService.Register(request);
                return Ok(response);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }

        }
    
        [Authorize]
        [HttpGet("getHindiName")]
        public async Task<ActionResult<string>> GetUserByUsername(string name)
        {
            var response = await _authenticationService.GetHindiName(name);
            if (response == null)
            {
                return NotFound("No name found");
            }
            return Ok(response);
        }


    }
}
