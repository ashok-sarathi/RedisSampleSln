using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RedisSaple.Data;
using RedisSaple.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace RedisSaple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(JwtContext context, IConfiguration configuration) : ControllerBase
    {
        [HttpGet("[action]")]
        public IActionResult SignIn(string email, string password)
        {
            var user = context.Set<User>().FirstOrDefault(x => x.Email == email);
            if (user == null)
                return Unauthorized(new
                {
                    details = "Invalid User"
                });

            var tokenHandler = new JwtSecurityTokenHandler();

            IList<Claim> claims = new List<Claim>()
            {
                new Claim("userid",user.Id.ToString()),
                new Claim("usertype",user.UserType.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JwtKey").Value)), SecurityAlgorithms.HmacSha256)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(tokenHandler.WriteToken(securityToken));
        }


        // POST api/<UserController>
        [HttpPost("[action]")]
        public void Register([FromBody] User user)
        {
            context.Set<User>().Add(user);
            context.SaveChanges();
        }
    }
}
