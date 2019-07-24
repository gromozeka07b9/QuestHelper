using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using QuestHelper.Managers.Sync;
using QuestHelper.View;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using QuestHelper.Managers;
using Xamarin.Forms;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using Xamarin.Essentials;

namespace QuestHelper
{
	public partial class App : Application
	{
	    //private bool SynchronizeStarted = false;
        private Logger _log = new Logger(true);

        public App ()
		{
			InitializeComponent();

		    _log.NewFile();
            MainPage = new View.MainPage();
		    Analytics.TrackEvent("Start app");
            Application.Current.Properties.Remove("SyncStatus");
		    Application.Current.Properties.Remove("WorkInRoaming");
		    ParameterManager par = new ParameterManager();
		    string showOnboarding = string.Empty;
		    if (!par.Get("NeedShowOnboarding", out showOnboarding))
		    {
		        par.Set("NeedShowOnboarding", "1");
		    }
		}

        protected override async void OnStart ()
		{
		    if (!AppCenter.Configured)
		    {
                Push.PushNotificationReceived += Push_PushNotificationReceived;
		    }

#if DEBUG
            AppCenter.Start("android=e7661afa-ce82-4b68-b98d-e35a71bb75c1;", typeof(Analytics), typeof(Crashes), typeof(Push));
#else
            AppCenter.Start("android=85c4ccc3-f315-427c-adbd-b928e461bcc8;", typeof(Analytics), typeof(Crashes), typeof(Push));
#endif

		    SubscribeMessages();

		    MessagingCenter.Subscribe<StartAppMessage>(this, string.Empty, (sender) =>
		    {
		        MainPage = new View.MainPage();
		    });

            TokenStoreService token = new TokenStoreService();

            if (Setup() && !string.IsNullOrEmpty(await token.GetAuthTokenAsync()))
            {
                Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(), string.Empty);
            }

        }

        private void Push_PushNotificationReceived(object sender, PushNotificationReceivedEventArgs e)
        {
            Analytics.TrackEvent("Push message: received", new Dictionary<string, string> { { "Message", e.Message } });
            string routeName = string.Empty;
            if (e.CustomData != null)
            {
                foreach (var key in e.CustomData.Keys)
                {
                    if (key.ToLower() == "routename")
                    {
                        routeName = e.CustomData[key];
                    }
                }
            }

            string questionText = string.Empty;
            if (!string.IsNullOrEmpty(routeName))
            {
                questionText = $"Вам доступен новый альбом: '{routeName}'\n Загрузить его сейчас?";
            }
            else
            {
                questionText = "Загрузить обновления сейчас?";
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Analytics.TrackEvent("Push message: show");
                bool needLoad = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Message = questionText, Title = "Обновление альбомов", OkText = "Да", CancelText = "Нет" });
                if (needLoad)
                {
                    Analytics.TrackEvent("Push message: start sync");
                    Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(), string.Empty);
                    var pageCollections = new PagesCollection();
                    MainPageMenuItem destinationPage = pageCollections.GetSelectAlbumsPage();
                    Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(new PageNavigationMessage() { DestinationPageDescription = destinationPage }, string.Empty);
                }
            });
        }

        private void SubscribeMessages()
	    {

	        MessagingCenter.Subscribe<MapOpenPointMessage>(this, string.Empty, (sender) =>
	        {
	            //пока непонятно, как будем открывать точку из карты
	            ShowWarning("Данная функция в разработке");
	        });

	        MessagingCenter.Subscribe<UIAlertMessage>(this, string.Empty,
	            (sender) =>
	            {
	                MainThread.BeginInvokeOnMainThread(() =>
	                {
	                    UserDialogs.Instance.Alert(sender.Message, sender.Title, "Ок");
	                });
	            });

	        MessagingCenter.Subscribe<UIToastMessage>(this, string.Empty, (sender) =>
	        {
	            MainThread.BeginInvokeOnMainThread(() =>
	            {
	                DependencyService.Get<IToastService>().LongToast(sender.Message);
	                //ToDo:Непонятно, чего тост не работает
	                //UserDialogs.Instance.Toast(sender.Message, new TimeSpan(0, 0, 0, 3));
	            });
	        });
	    }

	    private bool Setup()
        {
            bool result = false;

            RealmInstanceMaker realm = new RealmInstanceMaker();
            var realmInstance = realm.RealmInstance;

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
