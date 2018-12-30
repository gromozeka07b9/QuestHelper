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

        }

        protected override async void OnStart ()
		{
            AppCenter.Start("android=85c4ccc3-f315-427c-adbd-b928e461bcc8;", typeof(Analytics), typeof(Crashes));
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
	            SynchronizeStarted = true;
                DateTime startTime = DateTime.Now;
	            ShowWarning("Синхронизация запущена");
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
	            }
	            else
	            {
	                ShowWarning($"Длительность синхронизации:{DateTime.Now - startTime} сек.");
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
                ShowWarning("Изменена схема БД, нужно удалить приложение и установить новую версию. Это временно, починим.");
                Quit();
            }

            return result;
        }

        public void ShowWarning(string text)
        {            
            //MainPage.DisplayAlert("Внимание!", text, "Ok");
            DependencyService.Get<IToastService>().LongToast(text);

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
