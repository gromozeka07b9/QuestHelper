using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Auth;
using QuestHelper.Server.Managers;

namespace QuestHelper.Server
{
    public class RequestFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
            ValidateUser validateContext = new ValidateUser(_dbOptions, BearerParser.GetTokenFromHeader(context.HttpContext.Request.Headers));

            if (context.HttpContext.User.Identity.Name != null)
            {
                var userIdClaim = context.HttpContext.User.Claims.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault();
                if (!string.IsNullOrEmpty(userIdClaim.Value))
                {
                    context.HttpContext.Items.Add("UserId", userIdClaim.Value);
                    if (!validateContext.UserIsValid(context.HttpContext.User.Identity.Name))
                    {
                        context.Result = new StatusCodeResult(403);
                    }
                }
                else
                {
                    context.Result = new StatusCodeResult(403);
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
            base.OnActionExecuting(context);
        }
    }
}
