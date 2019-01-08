using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using QuestHelper.Managers.Sync;
using QuestHelper.View;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.Managers;
using Xamarin.Forms;
using QuestHelper.Model;
using QuestHelper.Model.Messages;

namespace QuestHelper
{
	public partial class App : Application
	{
	    private bool SynchronizeStarted = false;
		public App ()
		{
			InitializeComponent();


		    MainPage = new View.MainPage();
		    Analytics.TrackEvent("Start app");

        }

        protected override async void OnStart ()
		{
#if DEBUG
           AppCenter.Start("android=e7661afa-ce82-4b68-b98d-e35a71bb75c1;", typeof(Analytics), typeof(Crashes));
#else
           AppCenter.Start("android=85c4ccc3-f315-427c-adbd-b928e461bcc8;", typeof(Analytics), typeof(Crashes));
#endif
            MessagingCenter.Subscribe<SyncMessage>(this, string.Empty, async (sender) =>
		    {
		        await startSyncAll();
		    });

            TokenStoreService token = new TokenStoreService();

            if (Setup() && !string.IsNullOrEmpty(await token.GetAuthTokenAsync()))
            {
                await startSyncAll();
            }

        }

	    private async Task startSyncAll()
	    {
	        if (!SynchronizeStarted)
	        {
	            Analytics.TrackEvent("Sync all");
	            SynchronizeStarted = true;
                DateTime startTime = DateTime.Now;
	            //ShowWarning("Синхронизация запущена");
                var syncResult = await SyncServer.SyncAllAsync();
	            SynchronizeStarted = false;
                if (!syncResult.Item1)
	            {
	                if (SyncServer.AuthRequired)
	                {
	                    var pageCollections = new PagesCollection();
	                    MainPageMenuItem destinationPage = pageCollections.GetLoginPage();
	                    Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(
	                        new PageNavigationMessage() { DestinationPageDescription = destinationPage }, string.Empty);
	                }
	                else ShowWarning(syncResult.Item2);
	                Analytics.TrackEvent("Sync error", new Dictionary<string, string> { { "Sync error", syncResult.Item2 } });
	            }
                else
                {
                    var diff = DateTime.Now - startTime;
                    ShowWarning($"Длительность синхронизации:{diff} сек.");
	                Analytics.TrackEvent("Sync all done", new Dictionary<string, string>{{"Delay", Math.Round(diff.TotalSeconds).ToString()} });
	            }
            }
	        else ShowWarning($"Синхронизация уже запущена");
        }

        private bool Setup()
        {
            bool result = false;

            var realmInstance = RealmAppInstance.GetAppInstance();
            if (realmInstance != null)
            {
                string pathToPicturesDirectory = Path.Combine(DependencyService.Get<IPathService>().PrivateExternalFolder, "pictures");
                if (!Directory.Exists(pathToPicturesDirectory))
                {
                    Directory.CreateDirectory(pathToPicturesDirectory);
                }
                result = true;
            }
            else
            {
                Analytics.TrackEvent("MigrationRequired");
                ShowWarning("Изменена схема БД, нужно удалить приложение и установить новую версию. Это временно, починим.");
                Quit();
            }

            return result;
        }

        public void ShowWarning(string text)
        {            
            DependencyService.Get<IToastService>().ShortToast(text);

        }
        protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
