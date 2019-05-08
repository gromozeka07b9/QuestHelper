using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper.Managers.Sync;

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
            Console.WriteLine("SyncIntentService sync started");
            SyncServer syncSrv = new SyncServer();
            var syncResult = await syncSrv.SyncAll();
            Console.WriteLine("SyncIntentService sync ended");
        }
    }
}