using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [Authorize]
        [HttpGet("search/{TextForSearchUser}")]
        public IActionResult Get(string TextForSearchUser)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                string lowercaseTextForSearch = TextForSearchUser.ToLower();
                if (!string.IsNullOrEmpty(lowercaseTextForSearch))
                {
                    var FoundedUsers = db.User
                        .Where(s => s.Name.ToLower().Contains(lowercaseTextForSearch) ||
                                    s.Email.ToLower().Contains(lowercaseTextForSearch)).Select(s =>
                            new UserForSearch() { Name = s.Name, Email = s.Email, UserId = s.UserId }).ToList();
                    return new ObjectResult(FoundedUsers);
                }
                else
                {
                    return new ObjectResult(db.User.Select(s => new UserForSearch()
                        { Name = s.Name, Email = s.Email, UserId = s.UserId }).ToList());
                }
            }
        }
    }
}