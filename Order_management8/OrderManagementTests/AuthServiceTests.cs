using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Order_management.Models;
using Order_management.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order_management.DTOs;
using RegisterRequest = Order_management.DTOs.RegisterRequest;
using Order_management.Exceptions;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using LoginRequest = Order_management.DTOs.LoginRequest;
using NuGet.Protocol.Plugins;
namespace OrderManagementTests
{
    public class AuthServiceTests
    {
        private readonly Mock<IConfiguration> _config;

        public AuthServiceTests()
        {
            _config = new Mock<IConfiguration>(); 
        }

          private async Task<OrderManagementContext> GetDatabaseContext(int count = 2)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<OrderManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .UseInternalServiceProvider(serviceProvider);
            var context = new OrderManagementContext(optionsBuilder.Options);
           
            if (await context.Items.CountAsync() <= 0)
            {
                for (int i = 0; i < count; i++)
                {
                    context.Users.Add(sampleUsers[i]);
                    await context.SaveChangesAsync();
                }
            }

            return context;
        }
        public async void GetHashedPasswords(OrderManagementContext _mockContext,AuthService _authRepository)
        {
            
           var _users= await _mockContext.Users.ToListAsync();
           foreach( var u in _users)
            {
                u.Password = _authRepository.HashPassword(u.Password);
                _mockContext.SaveChanges();
            }
         
        }
        private List<User> sampleUsers = new List<User>()
        {
            new User{Id = 1,Username="maria",Email="maria@gmail.com",Password="Maria.123"},
            new User{Id = 2,Username="gigi",Email="gigi@gmail.com",Password="Gigi.123"},
        };
        [Fact]
        public async void Register_UserAlreadyExists_ThrowsException()
        {
            var _register=new RegisterRequest { Email = "maria@gmail.com", Username = "maria", Password = "Maria.123" };
            var _mockContext = await GetDatabaseContext();
            var _authRepository = new AuthService(_mockContext, _config.Object);

            await Assert.ThrowsAsync<ArgumentsException>(() => _authRepository.Register(_register));
        }
        [Fact]
        public async void Register_UserDoesntExist_AddsUser()
        {
            var _register = new RegisterRequest { Email = "yadu@gmail.com", Username = "yadu", Password = "Yadu.123" };
            var _mockContext = await GetDatabaseContext();
            var _authRepository = new AuthService(_mockContext, _config.Object);

            var result = await _authRepository.Register(_register);
            Assert.NotNull(result);
            Assert.IsType<User>(result);
        }
        [Fact]
        public async void Register_InvalidEmail_ThrowsException()
        {
            var _register = new RegisterRequest { Email = "maria@", Username = "maria", Password = "Maria.123" };
            var _mockContext = await GetDatabaseContext();
            var _authRepository = new AuthService(_mockContext, _config.Object);

           await Assert.ThrowsAsync<ArgumentsException>(() => _authRepository.Register(_register));
        }
        [Fact]
        public async void Register_InvalidUsername_ThrowsException()
        {
            var _register = new RegisterRequest { Email = "maria@gmail.com", Username = "M a", Password = "Maria.123" };
            var _mockContext = await GetDatabaseContext();
            var _authRepository = new AuthService(_mockContext, _config.Object);
           
            await Assert.ThrowsAsync<ArgumentsException>(() => _authRepository.Register(_register));
        }
        [Fact]
        public async void Register_InvalidPassword_ThrowsException()
        {
            var _register = new RegisterRequest { Email = "maria@gmail.com", Username = "maria", Password = "maria.123" };
            var _mockContext = await GetDatabaseContext();
            var _authRepository = new AuthService(_mockContext, _config.Object);

            await Assert.ThrowsAsync<ArgumentsException>(() => _authRepository.Register(_register));
        }
        [Fact]
        public async void GetUsers_NoUsersExist()
        {
            
            var _mockContext = await GetDatabaseContext(0);
            var _authRepository = new AuthService(_mockContext, _config.Object);

            var result = await _authRepository.GetUsers();
            Assert.Empty(result);
            Assert.IsType<List<User>>(result);
           

        }
        [Fact]
        public async void GetUsers_ReturnsUsers()
        {
           
            var _mockContext = await GetDatabaseContext();
            var _authRepository = new AuthService(_mockContext, _config.Object);

            var result = await _authRepository.GetUsers();
            Assert.NotEmpty(result);
            Assert.IsType<List<User>>(result);
            Assert.Equal(2, result.Count);
        }
        [Fact]
        public async void Login_UserDoesntExist_ThrowsException()
        {
            var _login = new LoginRequest { Username = "yadu", Password = "Yadu.123" };
            var _mockContext = await GetDatabaseContext();
            var _authRepository = new AuthService(_mockContext, _config.Object);                   

            await Assert.ThrowsAsync<ArgumentsException>(() => _authRepository.Login(_login));
        }
        [Fact]
        public async void Login_UserExistsAndValidPassword_ReturnsUser()
        {
            var _login = new LoginRequest { Username = "maria", Password = "Maria.123" };
            var _mockContext = await GetDatabaseContext();
           
            var _authRepository = new AuthService(_mockContext, _config.Object);

            GetHashedPasswords(_mockContext, _authRepository);

            var result = await _authRepository.Login(_login);
                
            Assert.NotNull(result);
            Assert.IsType<User>(result);
        }
        [Fact]
        public async void Login_UserExistsAndInvalidPassword_ThrowsException()
        {
            var _login = new LoginRequest { Username = "maria", Password = "Maria" };
            var _mockContext = await GetDatabaseContext();

            var _authRepository = new AuthService(_mockContext, _config.Object);
            GetHashedPasswords(_mockContext, _authRepository);
            await Assert.ThrowsAsync<ArgumentsException>(() => _authRepository.Login(_login));
        }
        [Fact]
        public async void ResetPassword_UserDoesntExist_ThrowsException()
        {
            var _reset = new ResetRequest { Username = "yadu", OldPassword = "Yadu.123",NewPassword="Yadu.1234" };
            var _mockContext = await GetDatabaseContext();
            var _authRepository = new AuthService(_mockContext, _config.Object);

            await Assert.ThrowsAsync<ArgumentsException>(() => _authRepository.ResetPassword(_reset));
        }
        [Fact]
        public async void ResetPassword_UserExistsAndValidPassword_ReturnsUser()
        {
            var _reset = new ResetRequest { Username = "maria", OldPassword = "Maria.123" ,NewPassword="Maria.1234"};
            var _mockContext = await GetDatabaseContext();

            var _authRepository = new AuthService(_mockContext, _config.Object);
            GetHashedPasswords(_mockContext, _authRepository);
            var result = await _authRepository.ResetPassword(_reset);

            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }
        [Fact]
        public async void ResetPassword_UserExistsAndInvalidPassword_ThrowsException()
        {
            var _reset = new ResetRequest { Username = "maria", OldPassword = "Maria", NewPassword = "Maria.1234" };
            var _mockContext = await GetDatabaseContext();

            var _authRepository = new AuthService(_mockContext, _config.Object);
            GetHashedPasswords(_mockContext, _authRepository);

            await Assert.ThrowsAsync<ArgumentsException>(() => _authRepository.ResetPassword(_reset));
        }
    }
}
