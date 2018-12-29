using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuestHelper.Server.Controllers
{
    [Route("/")]
    public class IndexController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "Welcome to GoSh! - I go, I share, I live.";
        }
    }
}
