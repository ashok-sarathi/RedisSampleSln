using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace RedisSaple.Helper
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        public Task Invoke(HttpContext httpContext)
        {
            try
            {
                var token = httpContext.Request.Headers.Authorization.FirstOrDefault()?.Split()?.Last();

                if (token != null)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    tokenHandler.ValidateToken(token, new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JwtKey").Value)),
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    } , out SecurityToken securityToken);
                    var jwtCode = (JwtSecurityToken)securityToken;

                    httpContext.Items.Add("userid", jwtCode.Claims.FirstOrDefault(p => p.Type == "userid")?.Value);
                    httpContext.Items.Add("usertype", jwtCode.Claims.FirstOrDefault(p => p.Type == "usertype")?.Value);
                }
            }
            catch { /* catch the exception is not required  */ }
            return next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
