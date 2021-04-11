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
using QuestHelper.Resources;
using QuestHelper.WS;

namespace QuestHelper
{
	public partial class App : Application
	{
        public static IContainer Container { get; set; }

        private ITextfileLogger _log;
        
        private const string _apiUrl = "http://igosh.pro";

	    static App()
	    {
	        InitializeIOCContainer();
	    }

	    private static void InitializeIOCContainer()
	    {
	        var builder = new ContainerBuilder();
            builder.RegisterInstance(new MemoryCache(new MemoryCacheOptions())).As<IMemoryCache>();
	        builder.RegisterInstance(new Logger(true)).As<ITextfileLogger>();
	        builder.RegisterInstance(new MediaFileManager()).As<IMediaFileManager>();
	        builder.Register((c,p) => new ServerRequest(_apiUrl)).As<IServerRequest>();
            Container = builder.Build();
	    }

        public App ()
		{
            Xamarin.Forms.Device.SetFlags(new string[] { "Shapes_Experimental" });
			Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("");
			
			InitializeComponent();
		    _log = App.Container.Resolve<ITextfileLogger>();

            _log.NewFile();
		    Analytics.TrackEvent("Start app");
            Application.Current.Properties.Remove("SyncStatus");
		    Application.Current.Properties.Remove("WorkInRoaming");
			ParameterManager par = new ParameterManager();
		    string showOnboarding = string.Empty;
		    if (!par.Get("NeedShowOnboarding", out showOnboarding))
		    {
				MainPage = new SplashWizardPage();
			}
			else
			{
				MainPage = new View.MainPage();
			}
		}

        protected override void OnStart()
        {

#if DEBUG
            AppCenter.Start("android=e7661afa-ce82-4b68-b98d-e35a71bb75c1;", typeof(Analytics), typeof(Crashes));
            //TokenStoreService tokenService = new TokenStoreService();
            //await tokenService.SetAuthDataAsync("", "");
            //ParameterManager par = new ParameterManager();
            //par.Set("GuestMode", "1");
#else
            AppCenter.Start("android=85c4ccc3-f315-427c-adbd-b928e461bcc8;", typeof(Analytics), typeof(Crashes));
#endif


            Setup();

            SubscribeMessages();
        }

        private void SubscribeMessages()
	    {

	        /*MessagingCenter.Subscribe<StartAppMessage>(this, string.Empty, (sender) =>
	        {
	            MainPage = new View.MainPage();
	        });*/

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

	        MessagingCenter.Subscribe<ReceiveTrackFile>(this, string.Empty, (sender) =>
	        {
		        string filename = sender.Filename;
		        MainThread.BeginInvokeOnMainThread(() =>
		        {
			        UserDialogs.Instance.Alert("Track file received", filename, "Ок");
			        ShowWarning("Track file received");
		        });

	        });
	        /*switch (Xamarin.Forms.Device.RuntimePlatform)
	        {
		        case Xamarin.Forms.Device.iOS:
			        Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(), string.Empty);
			        break;
		        default:
			        break;
	        }*/
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
                string pathToTracksDirectory = Path.Combine(DependencyService.Get<IPathService>().PrivateExternalFolder, "tracks");
                if (!Directory.Exists(pathToTracksDirectory))
                {
	                Directory.CreateDirectory(pathToTracksDirectory);
                }
                result = true;
            }
            else
            {
                Analytics.TrackEvent("MigrationRequired");
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
