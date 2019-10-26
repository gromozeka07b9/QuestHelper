﻿using System;
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

        [HttpPost]
        public async Task Token([FromBody]TokenRequest request)
        {
            using (var _db = new ServerDbContext(_dbOptions))
            {
                User user = _db.User.FirstOrDefault(x => x.Name == request.Username && x.Password == request.Password);
                if (user == null)
                {
                    user = _db.User.FirstOrDefault(x => x.Email == request.Username && x.Password == request.Password);
                }
                if (user != null)
                {
                    JwtResponseMaker jwtMaker = new JwtResponseMaker();
                    string response = jwtMaker.GetJwtResponse(user);
                    if (!string.IsNullOrEmpty(response))
                    {
                        Response.ContentType = "application/json";
                        await Response.WriteAsync(response);
                        Console.WriteLine($"Account login: status 200, {request?.Username}, {request?.Email}");
                    }
                    else
                    {
                        Response.StatusCode = 500;
                        await Response.WriteAsync("Error while generate token.");
                        Console.WriteLine($"Account login: status 500, {request?.Username}, {request?.Email}");
                    }

                }
                else
                {
                    Console.WriteLine($"Account Token: status 400, User not found in DB, {request?.Username}, {request?.Email}");
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
                    UserCreator userCreator = new UserCreator();
                    user = userCreator.Create(request, true);
                }
                JwtResponseMaker jwtMaker = new JwtResponseMaker();
                string response = jwtMaker.GetJwtResponse(user);
                if(!string.IsNullOrEmpty(response))
                {
                    Response.ContentType = "application/json";
                    await Response.WriteAsync(response);
                }
                else
                {
                    Response.StatusCode = 500;
                    await Response.WriteAsync("Error while generate token.");
                }
            }

            return;
        }

        [Route("new")]
        [HttpPost]
        public async Task New([FromBody]TokenRequest request)
        {
            using (var _db = new ServerDbContext(_dbOptions))
            {
                User user = _db.User.FirstOrDefault(x => x.Email == request.Email);
                if (user == null)
                {
                    UserCreator userCreator = new UserCreator();
                    user = userCreator.Create(request, false);
                    if (user != null)
                    {
                        userCreator.AddAccessToDemoRoute(user);
                        JwtResponseMaker jwtMaker = new JwtResponseMaker();
                        string response = jwtMaker.GetJwtResponse(user);
                        if (!string.IsNullOrEmpty(response))
                        {
                            Response.ContentType = "application/json";
                            await Response.WriteAsync(response);
                            Console.WriteLine($"Account New: status 200, {request?.Username}, {request?.Email}");
                        }
                        else
                        {
                            Response.StatusCode = 500;
                            await Response.WriteAsync("Error while generate token.");
                            Console.WriteLine($"Account New: status 500, {request?.Username}, {request?.Email}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Account New: status 403, {request?.Username}, {request?.Email}");
                    Response.StatusCode = 403;
                    await Response.WriteAsync("Error creating new user");
                }
            }

            return;
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