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
    [Route("api/account/[controller]")]
    public class GoogleController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpPost]
        public async Task Login([FromBody]OAuthTokenRequest request)
        {
            using (var _db = new ServerDbContext(_dbOptions))
            {
                UserCreator userCreator = new UserCreator();
                User user = _db.User.FirstOrDefault(x => x.Email == request.Email);
                if (user == null)
                {
                    user = userCreator.Create(new TokenRequest() {  Email = request.Email, Username = request.Username}, false);

                }

                if (user != null)
                {
                    OAuthUserCreator oauthUserCreator = new OAuthUserCreator();
                    var OauthUser = oauthUserCreator.CreateGoogleUser(request, user);
                    if(OauthUser != null)
                    {
                        userCreator.AddAccessToDemoRoute(user);
                        JwtResponseMaker jwtMaker = new JwtResponseMaker();
                        string response = jwtMaker.GetJwtResponse(user);
                        if (!string.IsNullOrEmpty(response))
                        {
                            Response.ContentType = "application/json";
                            await Response.WriteAsync(response);
                            Console.WriteLine($"Google account login: status 200, {request?.Username}, {request?.Email}");
                        }
                        else
                        {
                            Response.StatusCode = 500;
                            await Response.WriteAsync("Error while generate token.");
                            Console.WriteLine($"Google account login: status 500, {request?.Username}, {request?.Email}");
                        }
                    }
                    else
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync("Error while create login.");
                        Console.WriteLine($"Google account create login: status 500, {request?.Username}, {request?.Email}");
                    }
                }
                else
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync("Error while create user for Google.");
                    Console.WriteLine($"Google account create user: status 500, {request?.Username}, {request?.Email}");
                }
            }

            return;
        }
    }
}