using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper.Managers.Sync;
using QuestHelper.Model.Messages;
using Xamarin.Forms.Platform.Android;

namespace QuestHelper.Droid
{
    [Service]
    public class SyncIntentService : IntentService
    {
        public SyncIntentService() : base("SyncIntentService")
        {
        }

        protected override async void OnHandleIntent(Intent intent)
        {
            string routeId = intent.GetStringExtra("RouteId") ?? string.Empty;
            bool needCheckVersion = intent.GetBooleanExtra("NeedCheckVersionRoute", false);
            if (needCheckVersion)
            {
                bool needSyncRoute = await updateRouteIsNeeded(routeId);
                if (needSyncRoute)
                {
                    await startSync(routeId);
                }
            }
            else
            {
                await startSync(routeId);
            }
        }

        private async Task<bool> updateRouteIsNeeded(string routeId)
        {
            SyncServer syncSrv = new SyncServer();
            return await syncSrv.SyncRouteIsNeedAsync(routeId);
        }

        private static async Task startSync(string routeId)
        {
            //using (UserDialogs.Instance.Loading("Идет синхронизация данных...", () => { }, "", true, MaskType.Gradient))
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
}