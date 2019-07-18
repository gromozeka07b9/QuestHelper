using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using BottomNavigationBar;
using QuestHelper;
using Xamarin.Forms;
using QuestHelper.Model;
using System.Threading.Tasks;
using Android.Content;
using QuestHelper.Model.Messages;
using Plugin.CurrentActivity;
using ImageCircle.Forms.Plugin.Droid;
using Acr.UserDialogs;
using QuestHelper.Managers;
using Plugin.Permissions;
using QuestHelper.Managers.Sync;
using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;

namespace QuestHelper.Droid
{
    [Activity(Label = "QuestHelper", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, BottomNavigationBar.Listeners.IOnTabClickListener
    {

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void OnTabReSelected(int position)
        {
            NavigateToPage(position);
        }

        public void OnTabSelected(int position)
        {
            NavigateToPage(position);
        }

        private static void NavigateToPage(int position)
        {
            var pageCollections = new PagesCollection();
            MainPageMenuItem destinationPage = pageCollections.GetPageByPosition(position);
            Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>( new PageNavigationMessage() { DestinationPageDescription = destinationPage }, string.Empty);
            /*ParameterManager par = new ParameterManager();
            string NeedShowOnboarding = string.Empty;
            par.Get("NeedShowOnboarding", out NeedShowOnboarding);
            var pageCollections = new PagesCollection();
            if (NeedShowOnboarding.Equals("0"))
            {
                MainPageMenuItem destinationPage = pageCollections.GetPageByPosition(position);
                Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(
                    new PageNavigationMessage() { DestinationPageDescription = destinationPage }, string.Empty);
            }
            else
            {
                Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(
                    new PageNavigationMessage() { DestinationPageDescription = pageCollections.GetProcessWizardPage()}, string.Empty);
            }*/
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            CrossCurrentActivity.Current.Init(this, bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            ImageCircleRenderer.Init();
            UserDialogs.Init(this);

            string shareSubject = Intent.GetStringExtra("shareSubject") ?? string.Empty;
            string shareDescription = Intent.GetStringExtra("shareDescription") ?? string.Empty;

            LoadApplication(new App());
            if (!string.IsNullOrEmpty(shareDescription))
            {
                var pageCollections = new PagesCollection();
                MainPageMenuItem destinationPage = pageCollections.GetProcessSharePage();
                Xamarin.Forms.MessagingCenter.Send<ShareFromGoogleMapsMessage>(new ShareFromGoogleMapsMessage() { Subject = shareSubject, Description = shareDescription }, string.Empty);
            }
            else
            {
                StartToolbar(bundle);                
                ToolbarService bar  = new ToolbarService();
                bar.SetVisibilityToolbar(true);
            }

            MessagingCenter.Subscribe<SyncMessage>(this, string.Empty, async (sender) =>
            {
                SyncPossibility syncPossibility = new SyncPossibility();
                if (await syncPossibility.CheckAsync(true))
                {
                    Intent syncIntent = new Intent(this, typeof(SyncIntentService));
                    var result = StartService(syncIntent);
                }
            });

            //Используется для вывода нативного окна выбора oauth учетки
            MessagingCenter.Subscribe<OAuthDialogShowRequest>(this, string.Empty, (sender) =>
            {
                var intent = AuthenticationState.Authenticator.GetUI(Android.App.Application.Context);
                intent.SetFlags(ActivityFlags.NewTask);
                try
                {
                    Android.App.Application.Context.StartActivity(intent);
                }
                catch (Exception e)
                {
                    Xamarin.Forms.MessagingCenter.Send<UIAlertMessage>(new UIAlertMessage() { Title = "Авторизация Google", Message = "К сожалению, произошла ошибка при попытке использования Google Chrome" }, string.Empty);
                    Analytics.TrackEvent("Login OAuth error", new Dictionary<string, string> { { "ExceptionMessage", e.Message },{"Google Chrome auth", "error"} });
                    var pageCollections = new PagesCollection();
                    MainPageMenuItem destinationPage = pageCollections.GetLoginPage();
                    Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(new PageNavigationMessage() { DestinationPageDescription = destinationPage }, string.Empty);
                }
            });

        }

        private void StartToolbar(Bundle bundle)
        {
            ToolbarService.CreateToolbar(this, bundle);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (ToolbarService.Bar != null)
            {
                ToolbarService.Bar.OnSaveInstanceState(outState);
            }
        }
    }
}

