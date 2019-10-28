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
using Acr.UserDialogs;
using Autofac;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using QuestHelper.Managers;
using Xamarin.Forms;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using Xamarin.Essentials;

namespace QuestHelper
{
	public partial class App : Application
	{
        public static IContainer Container { get; set; }

        private ITextfileLogger _log;

	    static App()
	    {
	        InitializeIOCContainer();
	    }

	    private static void InitializeIOCContainer()
	    {
	        var builder = new ContainerBuilder();
            builder.RegisterInstance(new MemoryCache(new MemoryCacheOptions())).As<IMemoryCache>();
	        builder.RegisterInstance(new Logger(true)).As<ITextfileLogger>();
            Container = builder.Build();
	    }

        public App ()
		{
			InitializeComponent();
		    _log = App.Container.Resolve<ITextfileLogger>();

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

#if DEBUG
            AppCenter.Start("android=e7661afa-ce82-4b68-b98d-e35a71bb75c1;", typeof(Analytics), typeof(Crashes));
#else
            AppCenter.Start("android=85c4ccc3-f315-427c-adbd-b928e461bcc8;", typeof(Analytics), typeof(Crashes));
#endif

		    Setup();

		    SubscribeMessages();
		    
		}

        private void SubscribeMessages()
	    {

	        MessagingCenter.Subscribe<StartAppMessage>(this, string.Empty, (sender) =>
	        {
	            MainPage = new View.MainPage();
	        });

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

	        MessagingCenter.Subscribe<ReceivePushMessage>(this, string.Empty, (sender) =>
	        {
	            string messageBody = !string.IsNullOrEmpty(sender.MessageBody) ? sender.MessageBody : "Вам доступны новые маршруты и альбомы";
	            string messageTitle = sender.MessageTitle;
	            MainThread.BeginInvokeOnMainThread(() =>
	            {
	                UserDialogs.Instance.Alert(messageBody, messageTitle, "Ок");
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
