using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace QuestHelper.Server.Controllers
{
    [Route("/")]
    public class IndexController : Controller
    {
        // GET api/values
        [HttpGet]
        public string Get()
        {
            return "Backend for QuestHelper (GoSh!)";
        }
    }
}
