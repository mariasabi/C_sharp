using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Order_management.Interfaces;
using IAuthenticationService = Order_management.Interfaces.IAuthenticationService;
using log4net;
using Order_management.Service;
namespace Order_management.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private static readonly ILog log = LogManager.GetLogger(typeof(JwtMiddleware));
        public JwtMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;

        }

        public async Task Invoke(HttpContext context, IServiceProvider serviceProvider, IAuthenticationService userService)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();

                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (token != null)
                    attachUserToContext(context, authService, token);
            }
            await _next(context);
        }

        private void attachUserToContext(HttpContext context, IAuthenticationService authService, string token)
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

                context.Items["User"] = authService.GetById(userId);
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
    }
}
   
