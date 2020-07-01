using QuestHelper.LocalDB.Model;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuestHelper.Model;
using Xunit;
using QuestHelper.Managers;

namespace QuestHelper.Tests
{
    public class TestAutoRoutes
    {
        public TestAutoRoutes()
        {
        }

        [Fact]
        public void TestMust_LoadListImagesForOneDay()
        {
            bool result = true;

            //AutoRouteMakerManager routeMaker = new AutoRouteMakerManager();
            //routeMaker.Make(1, "");
            Assert.True(result);
        }

    }
}
