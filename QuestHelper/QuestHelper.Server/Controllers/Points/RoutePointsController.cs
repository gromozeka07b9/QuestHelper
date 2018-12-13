﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Points
{
    [Route("api/[controller]")]
    public class RoutePointsController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpGet("{RoutePointId}")]
        public IActionResult Get(string RoutePointId)
        {
            RoutePoint point = new RoutePoint();
            using (var db = new ServerDbContext(_dbOptions))
            {
                point = db.RoutePoint.SingleOrDefault(x => x.RoutePointId == RoutePointId);
            }
            return new ObjectResult(point);
        }

        [HttpPost]
        public void Post([FromBody]RoutePoint routePointObject)
        {
            using (var db = new ServerDbContext(_dbOptions))
            {
                var entity = db.RoutePoint.Find(routePointObject.RoutePointId);
                if (entity == null)
                {
                    db.RoutePoint.Add(routePointObject);
                } else
                {
                    db.Entry(entity).CurrentValues.SetValues(routePointObject);
                }
                db.SaveChanges();
            }
        }
    }
}