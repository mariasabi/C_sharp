using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Order_management.Interfaces;
using IAuthenticationService = Order_management.Interfaces.IAuthenticationService;
namespace Order_management.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        
        // private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, //IOptions<AppSettings> appSettings
                                      IConfiguration config )
        {
            _next = next;
            _config = config;
         
            //_appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context,IServiceProvider serviceProvider,IAuthenticationService userService)
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

        private void attachUserToContext(HttpContext context,IAuthenticationService authService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                //var key = Encoding.ASCII.GetBytes(_appSettings.Secret
                var key = Convert.FromBase64String(_config["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // attach user to context on successful jwt validation
                context.Items["User"] = authService.GetById(userId);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}
   
