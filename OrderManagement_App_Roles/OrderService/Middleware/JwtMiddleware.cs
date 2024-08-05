using log4net;
using Microsoft.IdentityModel.Tokens;
using OrderService.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text.Json;

namespace OrderService.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClient;
        private static readonly ILog log = LogManager.GetLogger(typeof(JwtMiddleware));
        public JwtMiddleware(RequestDelegate next, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _next = next;
            _config = config;
            _httpClient = httpClientFactory;

        }

        public async Task Invoke(HttpContext context)
        {

            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
             await  attachUserToContext(context, token);

            await _next(context);
        }

        private async Task attachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                var key = Convert.FromBase64String(_config["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                var user = await GetUserByIdAsync(userId);

                context.Items["User"] = user;
                //context.Items["User"] = _httpClient.GetAsync($"https://localhost:7044/api/User/getUser?id={userId}\r\n");
            }
            catch (SecurityTokenException ex)
            {
                log.Error("Token validation failed", ex);
            }
            catch (Exception ex)
            {
                log.Error("An error occurred when attaching user to context", ex);
            }
        }
          
        private async Task<User> GetUserByIdAsync(int userId)
        {
            var client = _httpClient.CreateClient();
           

            var response = await client.GetAsync($"https://localhost:7044/api/User/getUser?id={userId}");
            
                var userJson = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return user;
          
        
        }

        }
    }
    

      

