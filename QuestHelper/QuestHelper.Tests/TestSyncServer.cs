using System;
using System.Collections.Generic;
using System.Text;
using QuestHelper.Managers.Sync;
using QuestHelper.Model;
using Xunit;

namespace QuestHelper.Tests
{
    public class TestSyncServer
    {
        class TestSync : SyncBase
        {

        }

        [Fact]
        public void TestMust_SyncTestRoute()
        {
            string routeId = "";
            string authToken = "";
            SyncRoute syncRoute = new SyncRoute(routeId);
            syncRoute.SetAuthToken(authToken);
            Assert.True(syncRoute.Sync()); 
        }

        /*[Fact]
        public void TestMust_DetectObjectsForUpload()
        {
            TestSync sync = new TestSync();
            List<string> forUpload = new List<string>();
            List<string> forDownload = new List<string>();
            IEnumerable<Tuple<string, int>> clientObjects = new List<Tuple<string, int>>()
            {
                new Tuple<string, int>("id1", 1)//на клиенте один объект
            };
            SyncObjectStatus serverStatus = new SyncObjectStatus();
            var status1 = new SyncObjectStatus.ObjectStatus
            {
                ObjectId = "id1",
                Version = 0
            };
            serverStatus.Statuses.Add(status1);//этот объект сервер должен считать новым, клиент должен его передать
            var status2 = new SyncObjectStatus.ObjectStatus
            {
                ObjectId = "id2",
                Version = 1
            };
            serverStatus.Statuses.Add(status2);//этот объект уже есть на сервере, просто для шума добавил

            sync.FillListsObjectsForProcess(clientObjects, serverStatus, forUpload, forDownload);
            Assert.True(forUpload.Count == 1);
        }
        [Fact]
        public void TestMust_DetectObjectsForDownload()
        {
            TestSync sync = new TestSync();
            List<string> forUpload = new List<string>();
            List<string> forDownload = new List<string>();
            IEnumerable<Tuple<string, int>> clientObjects = new List<Tuple<string, int>>()
            {
                new Tuple<string, int>("id1", 1)//на клиенте только один объект
            };
            SyncObjectStatus serverStatus = new SyncObjectStatus();
            var status1 = new SyncObjectStatus.ObjectStatus
            {
                ObjectId = "id1",
                Version = 2
            };
            serverStatus.Statuses.Add(status1);//этот объект сервер должен считать более новым, версия на сервере выше чем на клиенте, поэтому клиент должен его забрать
            var status2 = new SyncObjectStatus.ObjectStatus
            {
                ObjectId = "id2",
                Version = 3
            };
            serverStatus.Statuses.Add(status2);//этого объекта на клиенте нет, поэтому клиент должен его забрать

            sync.FillListsObjectsForProcess(clientObjects, serverStatus, forUpload, forDownload);
            Assert.True(forDownload.Count == 2);
        }*/
    }
}
