﻿
using Microsoft.EntityFrameworkCore;
using Order_management.Interfaces;
using Order_management.Models;
using System.Security.Cryptography;
using System.Text;
using Order_management.DTOs;
using System.ComponentModel.DataAnnotations;
using log4net;
using Order_management.Exceptions;



namespace Order_management.Service
{
    public class AuthService: IAuthenticationService
    {
        private readonly OrderManagementContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthService));
        public AuthService(OrderManagementContext context)
        {
            _context = context;
        }
        public async Task<User> Register(RegisterRequest request)
        {
            ValidateEmail(request.Email);
            ValidateUsername(request.Username);
            ValidatePassword(request.Password);
            var userByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            var userByUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (userByEmail != null || userByUsername != null)
            {
                log.Debug($"Registration failed! User {request.Username} already exists.");
                throw new ArgumentsException($"Such a user already exists.");
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,     
                Password = HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            log.Info($"New user with user {request.Username} added.");
            return user;
        }

        public async Task<string> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || !VerifyPassword(request.Password, user.Password))
            {
                log.Debug($"Login failed! Username or password is invalid.");
                throw new ArgumentsException($"Login failed");
            }
            log.Info($"User {request.Username} logged in.");
            return "Login successful";
        }

        public async Task<string> ResetPassword(ResetRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || !VerifyPassword(request.OldPassword, user.Password))
            {
                log.Debug($"Unable to authenticate user for resetting password.");
                throw new ArgumentsException($"Username or old password is not valid.");
            }
            user.Password = HashPassword(request.NewPassword);
            _context.SaveChanges();
            log.Debug($"Password reset for user {request.Username}.");
            return "Password reset successfully";
        }
        public async Task<List<User>> GetUsers()
        {
            var user = await _context.Users.ToListAsync();

            if (user.Count==0)
            {
                log.Debug("No users found to retrieve.");
            }
           
            return user;
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
                throw new ArgumentsException("Invalid email format.");
            }
        }

        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                throw new ArgumentsException("Username must be at least 3 characters long.");

            }
        }
        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ArgumentsException("Password must be at least 6 characters long.");
            }

            if (!password.Any(char.IsUpper))
            {
                throw new ArgumentsException("Password must contain at least one uppercase letter.");
            }

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                throw new ArgumentsException("Password must contain at least one special character.");
            }
        }


    }
}

   
