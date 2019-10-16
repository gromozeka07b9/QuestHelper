using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.SharedModelsWS;

namespace QuestHelper.Server.Controllers
{
    /// <summary>
    /// Getting routes feed
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/[controller]")]
    public class FeedController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
        /// <summary>
        /// Get feed with filter
        /// </summary>
        /// <returns>List of routes</returns>
        [HttpGet]
        public IActionResult Get(FeedFilter filter)
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);

            string imageBaseUrl = $"{Request.Scheme}://{Request.Host.Value}/gallery";
            List<FeedItem> routes = new List<FeedItem>();
            using (var db = new ServerDbContext(_dbOptions))
            {
                routes = db.Route.Where(r => r.IsPublished && !r.IsDeleted)
                    .OrderBy(r=>r.CreateDate)
                    .Join(db.User, r=>r.CreatorId, u=>u.UserId, (r, u) => new {r,u})
                    .Select(n=>new FeedItem()
                    {
                        Id = n.r.RouteId,
                        Version = n.r.Version,
                        Name = n.r.Name,
                        CreateDate = n.r.CreateDate,
                        CreatorId = n.r.CreatorId,
                        CreatorName = n.u.Name,
                        ImgUrl = string.IsNullOrEmpty(n.r.ImgFilename) ? $"{imageBaseUrl}/default.jpg" : $"{imageBaseUrl}/{n.r.ImgFilename}",
                        Description = n.r.Description,
                        ItemType = FeedItemType.Route
                    }).ToList();
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"Get Feed: status 200, {userId}, delay:{delay.Milliseconds}");

            return new ObjectResult(routes);
        }
    }
}
