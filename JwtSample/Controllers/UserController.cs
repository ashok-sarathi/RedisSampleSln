using RedisSaple.Data;
using RedisSaple.Entity;
using RedisSaple.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RedisSaple.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(JwtContext context) : ControllerBase
    {
        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return context.Set<User>().AsEnumerable();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public User? Get(Guid id)
        {
            return context.Set<User>().Find(id);
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
