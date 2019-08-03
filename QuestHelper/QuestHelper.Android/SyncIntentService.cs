using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper.Managers.Sync;
using QuestHelper.Model.Messages;

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
            using (UserDialogs.Instance.Loading("Идет синхронизация данных...", () => { }, "", true, MaskType.Gradient))
            {
                Console.WriteLine("SyncIntentService sync started");
                SyncServer syncSrv = new SyncServer();
                var syncResult = await syncSrv.SyncAll();
                Console.WriteLine("SyncIntentService sync ended");
            }
        }
    }
}