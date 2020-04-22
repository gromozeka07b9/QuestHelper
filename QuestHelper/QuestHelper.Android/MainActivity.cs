
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Xamarin.Forms;
using Android.Content;
using QuestHelper.Model.Messages;
using Plugin.CurrentActivity;
using ImageCircle.Forms.Plugin.Droid;
using Acr.UserDialogs;
using Plugin.Permissions;
using QuestHelper.Managers.Sync;
using QuestHelper.Droid.Intents;
using QuestHelper.Consts;
using QuestHelper.Resources;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using System;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;

namespace QuestHelper.Droid
{
    [Activity(Label = "GoSh!", Icon = "@drawable/icon2", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        
        SyncPossibility _syncPossibility = new SyncPossibility();

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            CrossCurrentActivity.Current.Init(this, bundle);

            Forms.SetFlags("IndicatorView_Experimental");
            Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, bundle);

            Xamarin.FormsMaps.Init(this, bundle);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            ImageCircleRenderer.Init();
            Xamarin.Essentials.Platform.Init(this, bundle);
            UserDialogs.Init(this);

            PushReceiverSetup pushReceiverSetup = new PushReceiverSetup(this);
            pushReceiverSetup.Setup();

            string shareSubject = Intent.GetStringExtra("shareSubject") ?? string.Empty;
            string shareDescription = Intent.GetStringExtra("shareDescription") ?? string.Empty;

            DeviceSize.FullScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            DeviceSize.FullScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);

            LoadApplication(new App());

            /*if (!string.IsNullOrEmpty(shareDescription))
            {
                var pageCollections = new PagesCollection();
                MainPageMenuItem destinationPage = pageCollections.GetProcessSharePage();
                Xamarin.Forms.MessagingCenter.Send<ShareFromGoogleMapsMessage>(new ShareFromGoogleMapsMessage() { Subject = shareSubject, Description = shareDescription }, string.Empty);
            }*/

            MessagingCenter.Subscribe<SyncMessage>(this, string.Empty, async (sender) =>
            {
                if (await _syncPossibility.CheckAsync(true))
                {
                    Intent syncIntent = new Intent(this, typeof(SyncIntentService));
                    if ((sender != null) && !string.IsNullOrEmpty(sender.RouteId))
                    {
                        syncIntent.PutExtra("RouteId", sender.RouteId);
                        syncIntent.PutExtra("NeedCheckVersionRoute", sender.NeedCheckVersionRoute);
                    }
                    var result = StartService(syncIntent);
                }
            });

            MessagingCenter.Subscribe<AddRouteViewedMessage>(this, string.Empty, async (sender) =>
            {
                if (await _syncPossibility.CheckAsync(true))
                {
                    Intent intent = new Intent(this, typeof(SendRouteViewedIntentService));
                    intent.PutExtra("RouteId", sender.RouteId);
                    StartService(intent);
                }
            });

            MessagingCenter.Subscribe<SetEmotionRouteMessage>(this, string.Empty, async (sender) =>
            {
                if (await _syncPossibility.CheckAsync(true))
                {
                    Intent intent = new Intent(this, typeof(SetEmotionRouteIntentService));
                    intent.PutExtra("RouteId", sender.RouteId);
                    intent.PutExtra("Emotion", sender.Emotion);
                    StartService(intent);
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
                    Xamarin.Forms.MessagingCenter.Send<UIAlertMessage>(new UIAlertMessage() { Title = CommonResource.Login_GoogleAuthCaption, Message = CommonResource.Login_GoogleAuthError }, string.Empty);
                    Analytics.TrackEvent("Login OAuth error", new Dictionary<string, string> { { "ExceptionMessage", e.Message }, { "Google Chrome auth", "error" } });
                    Xamarin.Forms.MessagingCenter.Send<UIAlertMessage>(new UIAlertMessage() { Title = "Error", Message = "Error syncing server. Try to open feed." }, string.Empty);
                }
            });

        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            /*if (requestCode == 1)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                GoogleAuthManagerService.Instance.OnAuthCompleted(result);
            }*/
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }
    }
}

