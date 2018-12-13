using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuestHelper.Server.Auth;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Account
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        public class TokenRequest
        {
            public string Username;
            public string Password;
        }

        [HttpPost]
        public async Task Token([FromBody]TokenRequest request)
        {
            //var username = Request.Form["username"];
            //var password = Request.Form["password"];


            using (var _db = new ServerDbContext(_dbOptions))
            {
                User user = _db.User.FirstOrDefault(x => x.Name == request.Username && x.Password == request.Password);
                if (user != null)
                {
                    IdentityManager identityManager = new IdentityManager();
                    var identity = identityManager.GetIdentity(user);
                    if (identity != null)
                    {
                        JwtManager jwt = new JwtManager(_dbOptions);
                        var encodedJwt = jwt.GetEncodedJwt(identity);
                        jwt.WriteJwtHashToDb(user, encodedJwt);

                        var response = new
                        {
                            access_token = encodedJwt,
                            username = identity.Name
                        };

                        // сериализация ответа
                        Response.ContentType = "application/json";
                        await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
                    }
                    else
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync("Error while generate token.");
                    }
                }
                else
                {
                    Response.StatusCode = 400;
                    await Response.WriteAsync("Invalid username or password.");
                }
            }

            return;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            ValidateUser validateContext = new ValidateUser(_dbOptions, Request);
            if(!validateContext.UserIsValid(User.Identity.Name))
            {
                return Unauthorized();
            }
            return Ok($"Name:{User.Identity.Name}");
        }

        [Authorize(Roles = "admin")]
        [Route("getrole")]
        public IActionResult GetRole()
        {
            ValidateUser validateContext = new ValidateUser(_dbOptions, Request);
            if (!validateContext.UserIsValid(User.Identity.Name))
            {
                return Unauthorized();
            }
            return Ok("Ваша роль: администратор");
        }

    }
}