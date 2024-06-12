using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Order_management.Interfaces;
using Order_management.Models;
using System.Security.Cryptography;
using System.Text;
using Order_management.DTOs;
using System.ComponentModel.DataAnnotations;



//using Microsoft.AspNetCore.Identity.Data;

namespace Order_management.Service
{
    public class AuthService: IAuthenticationService
    {
        private readonly OrderManagementContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(OrderManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<string> Register(RegisterRequest request)
        {
            ValidateEmail(request.Email);
            ValidateUsername(request.Username);
            ValidatePassword(request.Password);
            var userByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            var userByUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (userByEmail != null || userByUsername != null)
            {
                throw new ArgumentException($"User with email {request.Email} or username {request.Username} already exists.");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,     
                Password = HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "Registration successful";
        }

        public async Task<string> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || !VerifyPassword(request.Password, user.Password))
            {
                throw new ArgumentException($"Unable to authenticate user {request.Username}");
            }

            return "Login successful";
        }

        public async Task<string> ResetPassword(ResetRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || !VerifyPassword(request.OldPassword, user.Password))
            {
                throw new ArgumentException($"Unable to authenticate user {request.Username}");
            }
            user.Password = HashPassword(request.NewPassword);
            _context.SaveChanges();
            return "Password reset";
        }
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashOfInput = HashPassword(inputPassword);
            return hashOfInput == storedHash;
        }
        private void ValidateEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                throw new ArgumentException("Invalid email format");
            }
        }

        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                throw new ArgumentException("Username must be at least 3 characters long");
            }
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ArgumentException("Password must be at least 6 characters long");
            }

            // Additional password complexity checks can be added here
        }

    }
}

   
