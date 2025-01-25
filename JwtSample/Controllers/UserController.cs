using RedisSaple.Data;
using RedisSaple.Entity;
using RedisSaple.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json.Serialization;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisSaple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(JwtContext context, IDistributedCache distributedCache) : ControllerBase
    {
        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return context.Set<User>().AsEnumerable();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var redisKey = "user_" + id.ToString();
            var cacheData = distributedCache.GetString(redisKey);
            if (cacheData is null)
            {
                var user = context.Set<User>().Find(id);
                if (user is not null)
                {
                    distributedCache.SetString(redisKey, JsonSerializer.Serialize(user), new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1)
                    });
                    return Ok(user);
                }
                return NotFound();
            }
            return StatusCode(StatusCodes.Status208AlreadyReported, JsonSerializer.Deserialize<User>(cacheData));
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] User user)
        {
            context.Set<User>().Update(user);
            context.SaveChanges();
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
            var data = context.Set<User>().FirstOrDefault(u => u.Id == id);
            if (data != null)
            {
                context.Set<User>().Remove(data);
                context.SaveChanges();
            }
        }
    }
}
