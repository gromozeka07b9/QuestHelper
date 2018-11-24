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

namespace QuestHelper
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            MainPage = new View.MainPage();
        }

        protected override void OnStart ()
		{
            AppCenter.Start("android=85c4ccc3-f315-427c-adbd-b928e461bcc8;", typeof(Analytics), typeof(Crashes));

		    if (Setup())
		    {
		        var syncPoints = SyncPoints.GetInstance();
		        syncPoints.SetWarningDialogContext(ShowWarning);

		        var syncFiles = SyncFiles.GetInstance();
		        syncFiles.SetWarningDialogContext(ShowWarning);

		        syncPoints.StartAsync();
		    }
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
