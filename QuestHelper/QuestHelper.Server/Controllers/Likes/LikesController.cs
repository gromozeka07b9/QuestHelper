using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.SharedModelsWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Controllers.Likes
{
    /// <summary>
    /// Like and dislike for routes/points
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/[controller]")]
    public class LikesController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpGet("/route/{routeId}")]
        public IActionResult RouteGetLikes(string routeId)
        {
            return new ObjectResult(routeId);
        }

        [HttpPost("{RouteId}/addemotion")]
        [ProducesResponseType(200)]
        public void AddEmotion([FromBody]Emotion EmotionStructure, string RouteId)
        {
            DateTime startDate = DateTime.Now;
            string userId = IdentityManager.GetUserId(HttpContext);

            using (var db = new ServerDbContext(_dbOptions))
            {
                var lastEmotion = db.RouteLike.Where(r => r.RouteId.Equals(RouteId) && r.UserId.Equals(userId)).OrderByDescending(r => r.SetDate).FirstOrDefault();
                if ((lastEmotion == null) || (( lastEmotion != null ) && ( !lastEmotion.IsLike.Equals(EmotionStructure.EmotionNum))))
                {
                    db.RouteLike.Add(new Models.RouteLike() { IsLike = EmotionStructure.EmotionNum, RouteId = RouteId, UserId = userId, SetDate = DateTime.Now });
                    db.SaveChanges();
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"Add emotion: status 200, {userId}, delay:{delay.TotalMilliseconds}");
        }

        public class Emotion
        {
            /// <summary>
            /// 0 - dislike, 1 - like
            /// </summary>
            public int EmotionNum { get; set; }
        }
    }
}
