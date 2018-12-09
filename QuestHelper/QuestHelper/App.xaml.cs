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

namespace QuestHelper
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();


		    MainPage = new View.MainPage();

		    /*var authService = DependencyService.Get<IAuthService>();
		    if (!string.IsNullOrEmpty(authService.GetAuthToken()))
		    {
		        MainPage = new View.LoginPage();
            }
		    else
		    {
		        MainPage = new View.MainPage();
		    }*/
            /*MessagingCenter.Subscribe<PageNavigationMessage>(this, string.Empty, (sender) => {
		        if (sender.DestinationPageDescription.TargetType == typeof(View.MainPage))
		        {
		            //MainPage = new View.MainPage();
		        } else if(sender.DestinationPageDescription.TargetType == typeof(View.LoginPage))

		        {
		            MainPage = new View.LoginPage();
		        }
            });*/

        }

        protected override async void OnStart ()
		{
            AppCenter.Start("android=85c4ccc3-f315-427c-adbd-b928e461bcc8;", typeof(Analytics), typeof(Crashes));

		    //var authService = DependencyService.Get<IAuthService>();

            /*if (Setup() && !string.IsNullOrEmpty(authService.GetAuthToken()))
		    {
                DateTime startTime = DateTime.Now;
                var syncResult = await SyncServer.SyncAllAsync();
		        if (!syncResult.Item1)
		        {
		            ShowWarning(syncResult.Item2);
		        }
		        ShowWarning($"Длительность синхронизации:{DateTime.Now - startTime} сек.");
            }*/

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
                ShowWarning("Требуется миграция данных, которой пока что нет. Сорри.");
                Quit();
            }

            return result;
        }

        public void ShowWarning(string text)
        {            
            MainPage.DisplayAlert("Внимание!", text, "Ok");
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
