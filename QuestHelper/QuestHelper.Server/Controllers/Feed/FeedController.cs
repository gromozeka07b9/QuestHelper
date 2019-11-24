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

            string imageBaseUrl = $"{Request.Scheme}://{Request.Host.Value}/shared";
            List<FeedItem> routes = new List<FeedItem>();
            using (var db = new ServerDbContext(_dbOptions))
            {
                string queryText = @"select r.RouteId, r.Name, r.Version, r.ImgFilename, r.CreateDate, r.CreatorId, u.Name as CreatorName, r.Description, ifnull(views.ViewCount, 0) as ViewCount, ifnull(likes.LikeCount,0) as LikeCount, ifnull(isViewed.Id, 0) as IsUserViewed, ifnull(isUserLikes.LikeCount, 0) as IsUserLikes from questhelper.Route as r 
                                        left join
                                        (
	                                        select l.RouteId, count(l.IsLike) as LikeCount from
	                                        (
	                                        SELECT RouteId, UserId, max(SetDate) as SetLikeDate FROM questhelper.RouteLike
	                                        group by RouteId, UserId
	                                        ) as q
	                                        left join questhelper.RouteLike as l
	                                        on q.RouteId = l.RouteId and q.UserId = l.UserId and q.SetLikeDate = l.SetDate
	                                        where l.IsLike = 1
	                                        group by l.RouteId
                                        ) likes
                                        on r.RouteId = likes.RouteId
                                        left join (
	                                        SELECT RouteId, count(ViewDate) as ViewCount FROM questhelper.RouteView
	                                        group by RouteId
                                        ) as views
                                        on r.RouteId = views.RouteId
                                        inner join questhelper.User as u
                                        on r.CreatorId = u.UserId
                                        left join questhelper.RouteView as isViewed
                                        on r.RouteId = isViewed.RouteId and isViewed.UserId = {0}
                                        left join(
                                            select l.RouteId, count(l.IsLike) as LikeCount from
                                            (
                                            SELECT RouteId, UserId, max(SetDate) as SetLikeDate FROM questhelper.RouteLike

                                            where UserId = {0}

                                            group by RouteId, UserId
                                            ) as q

                                            left join questhelper.RouteLike as l

                                            on q.RouteId = l.RouteId and q.UserId = l.UserId and q.SetLikeDate = l.SetDate

                                            where l.IsLike = 1

                                            group by l.RouteId
                                        ) as isUserLikes
                                        on r.RouteId = isUserLikes.RouteId
                                        where r.IsDeleted = 0 and r.IsPublished";
                var routesDbResult = db.FeedItem.FromSql(queryText, userId);
                var wsRoutes = from r in routesDbResult select new FeedItem() 
                { 
                    Id = r.RouteId,
                    CreatorId = r.CreatorId,
                    CreatorName = r.CreatorName,
                    CreateDate = r.CreateDate,
                    ImgUrl = string.IsNullOrEmpty(r.ImgFilename) ? tryToMakePreviewFromFirstPoint(r.RouteId, imageBaseUrl) : $"{imageBaseUrl}/{r.ImgFilename}",
                    Description = string.IsNullOrEmpty(r.Description) ? getDescriptionFromFirstPoint(r.RouteId) : r.Description,
                    LikeCount = r.LikeCount,
                    DislikeCount = 0,
                    ItemType = FeedItemType.Route,
                    ViewCount = r.ViewCount,
                    Name = r.Name,
                    Version = r.Version,
                    IsUserLiked = r.IsUserLikes,
                    IsUserViewed = r.IsUserViewed
                };
                routes = wsRoutes.ToList();
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"Get Feed: status 200, {userId}, delay:{delay.Milliseconds}");

            return new ObjectResult(routes);
        }

        private string tryToMakePreviewFromFirstPoint(string routeId, string imageBaseUrl)
        {
            Console.WriteLine($"Try to make preview: routeId:{routeId}");
            string previewUrl = $"{imageBaseUrl}/default.png";
            using (var db = new ServerDbContext(_dbOptions))
            {
                var firstPoint = db.RoutePoint.Where(p => p.RouteId.Equals(routeId) && !p.IsDeleted).OrderBy(p => p.CreateDate).FirstOrDefault();
                if (firstPoint != null)
                {
                    var firstMedia = db.RoutePointMediaObject.Where(m => m.RoutePointId.Equals(firstPoint.RoutePointId) && !m.IsDeleted).FirstOrDefault();
                    if (firstMedia != null)
                    {
                        string imgFileName = $"img_{firstMedia.RoutePointMediaObjectId}_preview.jpg";
                        MediaManager mediaManager = new MediaManager();
                        if(!mediaManager.SharedMediaFileExist(imgFileName))
                        {
                            Console.WriteLine($"Try to copy preview: imgFileName:{imgFileName}");
                            mediaManager.CopyMediaFileToSharedCatalog(imgFileName);
                        }
                        previewUrl = $"{imageBaseUrl}/{imgFileName}";
                    }
                }
            }
            return previewUrl;
        }
        private string getDescriptionFromFirstPoint(string routeId)
        {
            string description = string.Empty;

            using (var db = new ServerDbContext(_dbOptions))
            {
                var firstPoint = db.RoutePoint.Where(p => p.RouteId.Equals(routeId) && !p.IsDeleted).OrderBy(p => p.CreateDate).FirstOrDefault();
                if (firstPoint != null)
                {
                    description = firstPoint.Description;
                }
            }
            return description;
        }
    }
}
