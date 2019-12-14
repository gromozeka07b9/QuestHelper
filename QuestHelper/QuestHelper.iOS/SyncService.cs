using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Managers.Sync;

namespace QuestHelper.iOS
{
    public class SyncService 
    {
        public SyncService()
        {
        }

        public async void Start(string routeId, bool needCheckVersion)
        {
            if ((needCheckVersion) && !string.IsNullOrEmpty(routeId))
            {
                bool needSyncRoute = await updateRouteIsNeeded(routeId);
                if (needSyncRoute)
                {
                    await startCommonSync(routeId);
                }
            }
            else
            {
                await startCommonSync(routeId);
            }
        }

        private async Task<bool> updateRouteIsNeeded(string routeId)
        {
            SyncServer syncSrv = new SyncServer();
            return await syncSrv.SyncRouteIsNeedAsync(routeId);
        }

        private static async Task startCommonSync(string routeId)
        {
            Console.WriteLine("SyncIntentService sync started");
            SyncServer syncSrv = new SyncServer();
            bool syncResult;
            if (string.IsNullOrEmpty(routeId))
            {
                syncResult = await syncSrv.Sync();
            }
            else
            {
                syncResult = await syncSrv.Sync(routeId);
            }
            Console.WriteLine("SyncIntentService sync ended");
        }
    }
}