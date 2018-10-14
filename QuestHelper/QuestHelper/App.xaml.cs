using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using QuestHelper.Managers.Sync;
using QuestHelper.View;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var syncPoints = SyncPoints.GetInstance();
            syncPoints.SetWarningDialogContext(ShowWarning);
            Task.Run(()=>syncPoints.StartAsync());//deadlock
            syncPoints.StartAsync();

            var syncFiles = SyncFiles.GetInstance();
            syncFiles.SetWarningDialogContext(ShowWarning);
            syncFiles.Start();//ToDo:тоже надо асинк
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
