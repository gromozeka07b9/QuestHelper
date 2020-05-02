using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Controllers.v2
{
    /// <summary>
    /// CRUD for routes
    /// </summary>
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/v2/[controller]")]
    public class RoutesController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        /// <summary>
        /// List all available routes for user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery] PagingParameters pagingParameters)
        {
            int pageNumber = pagingParameters.IndexesRangeToPageNumber(pagingParameters.Range, pagingParameters.PageSize);
            int totalCountRows = 0;
            List<Route> items = new List<Route>();
            if(!string.IsNullOrEmpty(pagingParameters.Range))
            {
                string userId = IdentityManager.GetUserId(HttpContext);
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var routeaccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                    var withoutFilter = db.Route.Where(r => routeaccess.Contains(r.RouteId)).OrderBy(r => r.CreateDate);
                    totalCountRows = withoutFilter.Count();
                    items = withoutFilter.Skip((pageNumber - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).ToList();
                }
            }
            HttpContext.Response.Headers.Add("x-total-count", totalCountRows.ToString());
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
            return new ObjectResult(items);
        }

        [HttpGet("{RouteId}")]
        public IActionResult Get(string RouteId)
        {
            Route resultRoute = new Route();
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                RouteManager routeManager = new RouteManager(_dbOptions);
                resultRoute = routeManager.Get(userId, RouteId);
            }
            return new ObjectResult(resultRoute);
        }

        [HttpPut("{RouteId}")]
        public void Put(string RouteId, [FromBody]Route routeObject)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                var entity = db.Route.Find(routeObject.RouteId);
                if (entity == null)
                {
                    routeObject.CreatorId = userId;
                    routeObject.VersionsHash = string.Empty;
                    routeObject.VersionsList = string.Empty;
                    db.Route.Add(routeObject);
                    RouteAccess accessObject = new RouteAccess();
                    accessObject.UserId = userId;
                    accessObject.RouteId = routeObject.RouteId;
                    accessObject.RouteAccessId = Guid.NewGuid().ToString();
                    accessObject.CreateDate = DateTime.Now;
                    db.RouteAccess.Add(accessObject);
                }
                else
                {
                    if (!string.IsNullOrEmpty(entity.CreatorId))
                    {
                        if (!entity.CreatorId.Equals(routeObject.CreatorId))
                        {
                            //throw new Exception($"Gosh server: Client try to change CreatorId! was {entity.CreatorId} try {routeObject.CreatorId}");
                            //Непонятно, почему меняется CreatorId - он устанавливается только при создании маршрута
                            routeObject.CreatorId = entity.CreatorId;
                        }
                    }
                    routeObject.VersionsHash = string.Empty;
                    routeObject.VersionsList = string.Empty;
                    db.Entry(entity).CurrentValues.SetValues(routeObject);
                    /*if (!string.IsNullOrEmpty(routeObject.CoverImgBase64))
                    {
                        Base64Manager.SaveBase64ToFile(routeObject.CoverImgBase64, Path.Combine(_pathToMediaCatalog, routeObject.ImgFilename));
                    }*/
                }
                db.SaveChanges();
            }
        }

    }

    public class PagingParameters
    {
        const int maxPageSize = 50;
        private int _pageNumber = 1;
        private int _pageSize = 3;
        private string _range = string.Empty;

        public int PageNumber
        {
            get
            {
                return _pageNumber;
            }
        }

        public string Filter { get; set; } = string.Empty;

        public string Range
        {
            get
            {
                return _range;
            }
            set
            {
                _range = value;
            }
        }

        public int IndexesRangeToPageNumber(string range, int pageSize)
        {
            //[0,9]=1,[10,19]=2,PageSize=10
            int idxFrom, idxTo = 0;
            var arrIdx = range.Replace("[", "").Replace("]", "").Split(",");
            if(arrIdx.Length == 2)
            {
                idxFrom = Convert.ToInt32(arrIdx[0]);
                idxTo = Convert.ToInt32(arrIdx[1]);
                if(idxFrom == 0)
                {
                    return 1;
                }
                else
                {
                    return (idxFrom / pageSize) + 1;
                }
            }
            else
            {
                return 1;
            }
        }

        public string Sort { get; set; } = string.Empty;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
