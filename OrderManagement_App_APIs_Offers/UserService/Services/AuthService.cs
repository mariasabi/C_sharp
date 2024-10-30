  using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using UserService.Exceptions;
using System.Security.Cryptography;
using UserService.DTOs;
using UserService.Models;
using Microsoft.EntityFrameworkCore;
using log4net;
using UserService.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Caching.Memory;
namespace UserService.Services
{
    public class AuthService : Interfaces.IAuthenticationService
    {
        private readonly OrderContext _context;
        private readonly IConfiguration _config;
        private readonly IMemoryCache _cache;
        private readonly IEmailSender _emailSender;
        private static readonly ILog log = LogManager.GetLogger(typeof(AuthService));
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(IMemoryCache cache,OrderContext context, IConfiguration config, IHttpContextAccessor http,IEmailSender emailSender)
        {
            _cache = cache;
            _context = context;
            _config = config;
            _httpContextAccessor = http;

            _emailSender = emailSender;
        }
        /// <summary>
        /// Register user giving email, username and password. 
        /// Validate these parameters.
        /// Regsitration is failed if user already exists.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<User></returns>
        /// <exception cref="ArgumentsException"></exception>
        public async Task<User> Register(Register request)
        {
            ValidateEmail(request.Email);
            ValidateUsername(request.Username);
            ValidatePassword(request.Password);
            var userByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            var userByUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if ((userByEmail != null && userByEmail.Deleted == false) || (userByUsername != null && userByUsername.Deleted == false))
            {
                  log.Debug($"Registration failed! User {request.Username} already exists.");
                  throw new ArgumentsException($"Such a user already exists.");
                
            }
          
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = HashPassword(request.Password),
                Role= "User",
                HindiName=request.HindiName
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
        public async Task<User> Login( Login request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || user.Deleted||!VerifyPassword(request.Password, user.Password))
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

        public string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()),
                    new Claim("username",user.Username.ToString()),
                    new Claim("role",user.Role.ToString())}), 
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<string> ForgotPassword(ForgotPassword model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null || user.Deleted)
            {
                throw new ArgumentException("If the email is registered, you will receive a password reset link.");
            }

          
            var otp = GenerateOtp();
            StoreOtpInCache(user.Email, otp);

         
            await _emailSender.SendEmailAsync(user.Email, "OTP for Quick Buy",
                $"Your OTP for password reset is: {otp}. This OTP is valid for 5 minutes.");

            return "Password reset OTP has been sent to your email.";
        }
        public string ValidateOTP(string email, string otp)
        {
            var cacheKey = GetOtpCacheKey(email);
            if (_cache.TryGetValue(cacheKey, out string cachedOtp) && cachedOtp == otp)
                return "Valid OTP";
            else
                throw new ArgumentsException("Invalid or expired OTP");
        }

        private void StoreOtpInCache(string email, string otp)
        {
            var cacheKey = GetOtpCacheKey(email);
            _cache.Set(cacheKey, otp, TimeSpan.FromMinutes(5));
        }

        private string GetOtpCacheKey(string email) => $"OTP_{email}";

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); 
        }
        /// <summary>
        /// Reset password if username/email and old password matches.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<string></returns>
        /// <exception cref="ArgumentsException"></exception>
        public async Task<string> ResetPassword(Reset request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || user.Deleted)
            {
                throw new ArgumentsException("User not found or deleted.");
            }

            user.Password = HashPassword(request.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Clear OTP from cache
            _cache.Remove(GetOtpCacheKey(request.Email));

            return "Password has been successfully reset.";
        }
 
        /// <summary>
        /// Create a hash of the password using SHA256 encoding.
        /// </summary>
        /// <param name="password"></param>
        /// <returns>string</returns>
        public string HashPassword(string password)
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
        public async Task<string> GetHindiName(string name)
        {
            var user = _context.Users.FirstOrDefault(_x => _x.Username == name);
            if (user.Deleted == false) { return user.HindiName; }
            else
                return null;
        }
    }
}
