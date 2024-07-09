//using Microsoft.AspNetCore.Identity.Data;
using Moq;
using Order_management.Controllers;
using Order_management.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order_management.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.Data;
using Order_management.Models;
using LoginRequest = Order_management.DTOs.LoginRequest;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using RegisterRequest = Order_management.DTOs.RegisterRequest;
namespace OrderManagementTests
{
    public class UserControllerTests
    {
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly UserController _controller;
        public UserControllerTests()
        {
            _mockAuthService = new Mock<IAuthenticationService>();
            _controller = new UserController(_mockAuthService.Object);
        }
       
       
        [Fact]
        public async Task Login_TokenGenerated()
        {
            User _user = new User { Email = "maria@gmail.com", Username = "maria_sabi", Password = "maria.123" };
            LoginRequest _login = new LoginRequest { Username = "maria_sabi", Password = "maria.123" };
            string response="Sometoken";
            _mockAuthService.Setup(x => x.Login(_login)).ReturnsAsync(_user);
            _mockAuthService.Setup(x => x.GenerateJSONWebToken(It.IsAny<User>())).Returns(response);
            var result = await _controller.Login(_login);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<string>(okResult.Value);
        }
        
        [Fact]
        public async Task Register_RegisteredUser()
        {
            User _user = new User { Email = "maria@gmail.com", Username = "maria", Password = "maria.123" };
            RegisterRequest _register = new RegisterRequest { Email="maria@gmail.com",Username = "maria", Password = "maria.123" };
           
            _mockAuthService.Setup(x => x.Register(_register)).ReturnsAsync(_user);
            
            var result = await _controller.Register(_register);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<User>(okResult.Value);
            
        }
        [Fact]
        public async Task ResetPassword_UpdatedUser()
        {

            ResetRequest _reset = new ResetRequest { Username = "maria", OldPassword = "maria.123", NewPassword = "maria.1234" };

            _mockAuthService.Setup(x => x.ResetPassword(_reset)).ReturnsAsync("Password reset successfully");

            var result = await _controller.ResetPassword(_reset);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<string>(okResult.Value);
        }
        [Fact]
        public async Task GetUsers_ReturnOneOrMoreUsers()
        {
            var _users= new List<User>()
            {
                new User { Email = "maria@gmail.com", Username = "maria", Password = "maria.1234" },
                new User { Email = "gigi@gmail.com", Username = "gigi", Password = "gigi.1234" }
            };
            User _user = new User { Email = "maria@gmail.com", Username = "maria", Password = "maria.1234" };
            
            _mockAuthService.Setup(x => x.GetUsers()).ReturnsAsync(_users);
            
            var result = await _controller.GetUsers();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<User>>(okResult.Value);
           
        }
        [Fact]
        public async Task GetUsers_ReturnZeroUsers()
        {
            
            _mockAuthService.Setup(x => x.GetUsers()).ReturnsAsync(new List<User> { });          
            var result = await _controller.GetUsers();            
            var okResult = Assert.IsType<NotFoundObjectResult>(result.Result);
          
        }
    }

    
}
