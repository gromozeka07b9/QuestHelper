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
            using (var _db = new ServerDbContext(_dbOptions))
            {
                User user = _db.User.FirstOrDefault(x => x.Name == request.Username && x.Password == request.Password);
                if (user != null)
                {
                    IdentityManager identityManager = new IdentityManager();
                    var identity = identityManager.GetIdentity(user);
                    if (identity != null)
                    {
                        JwtManager jwt = new JwtManager();
                        var encodedJwt = jwt.GetEncodedJwt(identity, user.TokenKey);

                        var response = new
                        {
                            access_token = encodedJwt,
                            username = user.Name
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

        [Route("demotoken")]
        [HttpPost]
        public async Task DemoToken([FromBody]TokenRequest request)
        {
            using (var _db = new ServerDbContext(_dbOptions))
            {
                User user = _db.User.FirstOrDefault(x => x.Name == request.Username && x.Role == "demo");
                if (user == null)
                {
                    user = createDemoUser(request, _db);
                }

                IdentityManager identityManager = new IdentityManager();
                var identity = identityManager.GetIdentity(user);
                if (identity != null)
                {
                    JwtManager jwt = new JwtManager();
                    var encodedJwt = jwt.GetEncodedJwt(identity, user.TokenKey);

                    var response = new
                    {
                        access_token = encodedJwt,
                        username = identity.Name
                    };

                    // сериализация ответа
                    Response.ContentType = "application/json";
                    await Response.WriteAsync(JsonConvert.SerializeObject(response,
                        new JsonSerializerSettings {Formatting = Formatting.Indented}));
                }
                else
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync("Error while generate token.");
                }
            }

            return;
        }

        private User createDemoUser(TokenRequest request, ServerDbContext _db)
        {
            User demoUser = new User();
            demoUser.UserId = Guid.NewGuid().ToString();
            demoUser.Role = "demo";
            demoUser.Name = request.Username;
            demoUser.Password = demoUser.Name;
            demoUser.Version = 1;
            demoUser.CreateDate = DateTime.Now;
            demoUser.TokenKey = demoUser.Name;
            _db.User.Add(demoUser);
            _db.SaveChanges();
            return demoUser;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            ValidateUser validateContext = new ValidateUser(_dbOptions, BearerParser.GetTokenFromHeader(Request.Headers));
            if(!validateContext.UserIsValid(User.Identity.Name))
            {
                return Unauthorized();
            }
            return Ok($"Name:{User.Identity.Name}");
        }
    }
}