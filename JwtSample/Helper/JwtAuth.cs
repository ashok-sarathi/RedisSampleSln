using RedisSaple.Data;
using RedisSaple.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RedisSaple.Helper
{
    public class JwtAuth : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Items.FirstOrDefault(p => p.Key.ToString() == "userid")!.Value?.ToString();
            var userType = context.HttpContext.Items.FirstOrDefault(p => p.Key.ToString() == "usertype")!.Value?.ToString();

            if (userId == null)
            {
                context.Result = GetUnAuthResult();
            }
            else
            {
                if (userType != UserType.Admin.ToString())
                {
                    switch (context.HttpContext.Request.Method)
                    {
                        case "POST":
                        case "PUT":
                        case "DELETE":
                            context.Result = GetUnAuthResult();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private JsonResult GetUnAuthResult()
        {
            return new JsonResult(new { message = "Unauthorized" })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }
    }
}
