using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
//using QuestHelper.SharedModelsWS;

namespace QuestHelper.Server.Controllers.v2.Public
{
    /// <summary>
    /// Контроллер задуман для доступа без авторизации к публичным маршрутам - они должны иметь внешнюю ссылку и быть опубликованы в ленте
    /// </summary>
    [Route("api/v2/public/[controller]")]
    public class RoutesController : Controller
    {

        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
        private MediaManager _mediaManager;
        private string _pathToMediaCatalog;

        public RoutesController()
        {
            _mediaManager = new MediaManager();
            _pathToMediaCatalog = _mediaManager.PathToMediaCatalog;
        }

        /// <summary>
        /// Список всех маршрутов доступных без авторизации.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery] PagingParameters pagingParameters)
        {
            FilterParameters filters = new FilterParameters(pagingParameters.Filter);
            int pageNumber = pagingParameters.IndexesRangeToPageNumber(pagingParameters.Range, pagingParameters.PageSize);
            int totalCountRows = 0;
            List<Route> items = new List<Route>();
            if (!string.IsNullOrEmpty(pagingParameters.Range))
            {
                using (var db = new ServerDbContext(_dbOptions))
                {

                    var sharedRoutes = db.RouteShare.Select(s => s.RouteId).ToList();
                    var withoutFilter = db.Route.Where(r => !r.IsDeleted && r.IsPublished && sharedRoutes.Contains(r.RouteId));

                    withoutFilter = filters.isFilterPresent("createDate") ? withoutFilter.Where(r => r.CreateDate.Equals(filters.GetDateTimeByName("createDate"))) : withoutFilter;
                    withoutFilter = filters.isFilterPresent("creatorId") ? withoutFilter.Where(r => r.CreatorId.Contains(filters.GetStringByName("creatorId"))) : withoutFilter;
                    withoutFilter = filters.isFilterPresent("name") ? withoutFilter.Where(r => r.Name.Contains(filters.GetStringByName("name"))) : withoutFilter;
                    withoutFilter = filters.isFilterPresent("description") ? withoutFilter.Where(r => r.Description.Contains(filters.GetStringByName("description"))) : withoutFilter;
                    if (filters.isFilterPresent("createDate"))
                    {
                        var cd = filters.GetDateTimeByName("createDate");
                        withoutFilter = withoutFilter.Where(r => r.CreateDate.Year.Equals(cd.Year) && r.CreateDate.Month.Equals(cd.Month) && r.CreateDate.Day.Equals(cd.Day));
                    }

                    totalCountRows = withoutFilter.Count();
                    items = withoutFilter.OrderBy(r => r.CreateDate).Skip((pageNumber - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).ToList();
                }
            }
            HttpContext.Response.Headers.Add("x-total-count", totalCountRows.ToString());
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-total-count");
            return new ObjectResult(items);
        }
    }
}
