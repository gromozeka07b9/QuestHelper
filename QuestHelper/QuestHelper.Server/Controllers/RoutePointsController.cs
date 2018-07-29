using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers
{
    [Route("api/[controller]")]
    public class RoutePointsController : Controller
    {
        [HttpGet("{RouteId}")]
        public IActionResult Get(int RouteId)
        {
            List<RoutePoint> items = new List<RoutePoint>();
            using (var db = new ServerDbContext())
            {
                items = db.RoutePoint.Where(x=>x.RouteId == RouteId.ToString()).ToList();
            }
            return new ObjectResult(items);
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
    }
}
