using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers
{
    [Route("api/[controller]")]
    public class RoutesController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            List<Route> items = new List<Route>();
            using (var db = new ServerDbContext())
            {
                items = db.Route.ToList();
            }
            return new ObjectResult(items);
        }
    }
}
