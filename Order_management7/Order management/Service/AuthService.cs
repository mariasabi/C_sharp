
using Microsoft.EntityFrameworkCore;
using Order_management.Interfaces;
using Order_management.Models;
using System.Security.Cryptography;
using System.Text;
using Order_management.DTOs;
using System.ComponentModel.DataAnnotations;
using log4net;
using Order_management.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;



namespace Order_management.Service
{
    public class AuthService : IAuthenticationService
    {
        private readonly OrderManagementContext _context;
        private readonly IConfiguration _config;
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthService));
        public AuthService(OrderManagementContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        /// <summary>
        /// Register user giving email, username and password. 
        /// Validate these parameters.
        /// Regsitration is failed if user already exists.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<User></returns>
        /// <exception cref="ArgumentsException"></exception>
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
        /// <summary>
        /// Login credentials are username/email and password.
        /// Login is failed if user doesn't exist.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentsException"></exception>
        public async Task<User> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || !VerifyPassword(request.Password, user.Password))
            {
                log.Debug($"Login failed! Username or password is invalid.");
                throw new ArgumentsException($"Login failed");
            }
            log.Info($"User {request.Username} logged in.");
            return user;
        }
        /// <summary>
        /// JSON web token is generated with credentials created using key give, claims 
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns>string</returns>
        public string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.UtcNow.AddMinutes(10), 
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        /// <summary>
        /// Reset password if username/email and old password matches.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<string></returns>
        /// <exception cref="ArgumentsException"></exception>
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
        /// <summary>
        /// Retrieve all users.
        /// </summary>
        /// <returns>Task<List<User>></returns>
        public async Task<List<User>> GetUsers()
        {
            var user = await _context.Users.ToListAsync();

            if (user.Count == 0)
            {
                log.Debug("No users found to retrieve.");
            }

            return user;
        }
        /// <summary>
        /// Create a hash of the password using SHA256 encoding.
        /// </summary>
        /// <param name="password"></param>
        /// <returns>string</returns>
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
        /// <summary>
        /// Verify if the password hash and stored password hash matches.
        /// </summary>
        /// <param name="inputPassword"></param>
        /// <param name="storedHash"></param>
        /// <returns>bool</returns>
        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashOfInput = HashPassword(inputPassword);
            return hashOfInput == storedHash;
        }
        /// <summary>
        /// Validate email format.
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="ArgumentsException"></exception>
        private void ValidateEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                throw new ArgumentsException("Invalid email format.");
            }
        }
        /// <summary>
        /// Validate if username is atleast 3 characters long
        /// </summary>
        /// <param name="username"></param>
        /// <exception cref="ArgumentsException"></exception>
        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                throw new ArgumentsException("Username must be at least 3 characters long.");

            }
        }
        /// <summary>
        /// Validate if password is at least 6 characters long and has at least one special character and uppercase letter.
        /// </summary>
        /// <param name="password"></param>
        /// <exception cref="ArgumentsException"></exception>
        private void ValidatePassword(string password)
        {
            Regex exp = new Regex(@"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{6,}$");
            if (!exp.IsMatch(password))
            {
                throw new ArgumentsException("Password must be at least 6 characters long and must have at least one special character and uppercase letter.");
            }

        }
    }
}

   
