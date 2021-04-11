
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
using System.IO;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;
using Xamarin.Auth;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Provider;
using Java.IO;
using QuestHelper.Managers;
using File = Java.IO.File;

namespace QuestHelper.Droid
{
    [IntentFilter(new[] { Intent.ActionView, Intent.ActionEdit, Intent.ActionSend, Intent.ActionMain }, Label = "Gosh!", Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataMimeType = "text/plain")]
    [IntentFilter(new[] { Intent.ActionView, Intent.ActionEdit, Intent.ActionSend, Intent.ActionMain }, Label = "Gosh!", Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataMimeType = "*/*")]
#if DEBUG
    [Activity(Label = "Gosh! Debug", Icon = "@drawable/icon2", Theme = "@style/MainTheme", MainLauncher = true, NoHistory = false, LaunchMode = LaunchMode.SingleTask, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
#else
    [Activity(Label = "Gosh!", Icon = "@drawable/icon2", Theme = "@style/MainTheme", MainLauncher = true, NoHistory = false, LaunchMode = LaunchMode.SingleTask, ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
#endif
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private string shareSubject = string.Empty;
        private string shareDescription = string.Empty;

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

            this.Window.DecorView.Background = null;
            this.Window.DecorView.SetBackgroundColor(Android.Graphics.Color.White);

            CrossCurrentActivity.Current.Init(this, bundle);

            //Forms.SetFlags("SwipeView_Experimental");
            //Forms.SetFlags("IndicatorView_Experimental");
            //Forms.SetFlags("CollectionView_Experimental");
            global::Xamarin.Forms.Forms.Init(this, bundle);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            //https://github.com/roubachof/Xamarin.Forms.Nuke
#if DEBUG
            Android.Glide.Forms.Init(this, debug:true);
#else
            Android.Glide.Forms.Init(this);
#endif

            Xamarin.FormsMaps.Init(this, bundle);
            ImageCircleRenderer.Init();
            Xamarin.Essentials.Platform.Init(this, bundle);
            UserDialogs.Init(this);
            CustomTabsConfiguration.CustomTabsClosingMessage = null;

            PushReceiverSetup pushReceiverSetup = new PushReceiverSetup(this);
            pushReceiverSetup.Setup();

            string shareSubject = Intent.GetStringExtra("shareSubject") ?? string.Empty;
            string shareDescription = Intent.GetStringExtra("shareDescription") ?? string.Empty;

            DeviceSize.FullScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
            DeviceSize.FullScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);

            LoadApplication(new App());

            if (Intent?.Extras != null)
            {
                if (Intent.Extras?.KeySet()?.Count > 0)
                {
                    //ToDo: несмотря на передачу extra в FirebaseNotificationService они почему-то не передаются
                    //Актуально когда гош открываешь из сообщения в шторке
                    string messageBody = Intent.Extras.GetString("messageBodyText");
                    Xamarin.Forms.MessagingCenter.Send<ReceivePushMessage>(new ReceivePushMessage() { MessageBody = messageBody, MessageTitle = string.Empty }, string.Empty);
                }
            }

            /*if (Intent != null)
            {
                processShareIntent(Intent);
            }*/

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

        protected override void OnNewIntent(Intent intent)
        {
            //if (intent?.ClipData?.ItemCount > 0)
            //var file = intent.ClipData.GetItemAt(0);
            var fileUri = intent?.Data;
            if (fileUri != null)
            {
                var cursor = ContentResolver?.Query(fileUri, null, null, null, null);
                string filename = String.Empty;
                if (cursor != null && cursor.MoveToFirst())
                {
                    filename = cursor.GetString(cursor.GetColumnIndex(OpenableColumns.DisplayName));
                }
                if (!string.IsNullOrEmpty(filename))
                {
                    var fileStream = ContentResolver?.OpenInputStream(fileUri);
                    var memoryStream = new MemoryStream();
                    fileStream?.CopyTo(memoryStream);
                    //string filename = fileUri.LastPathSegment??string.Empty;
                    if (!string.IsNullOrEmpty(filename))
                    {
                        System.IO.File.WriteAllBytes(System.IO.Path.Combine(ImagePathManager.GetTracksDirectory(), filename), memoryStream.ToArray());
                        Xamarin.Forms.MessagingCenter.Send<ReceiveTrackFile>(new ReceiveTrackFile() { Filename = filename}, string.Empty);
                    }
                    else
                    {
                        Xamarin.Forms.MessagingCenter.Send<UIAlertMessage>(new UIAlertMessage() { Title = CommonResource.CommonMsg_Warning, Message = $"Error while loading track file [{fileUri}]" }, string.Empty);
                    }
                }
                else
                {
                    Xamarin.Forms.MessagingCenter.Send<UIAlertMessage>(new UIAlertMessage() { Title = CommonResource.CommonMsg_Warning, Message = $"Error while loading track file [{fileUri}]" }, string.Empty);
                }
            }
        }
        /*private void processShareIntent(Intent shareIntent)
        {
            switch (shareIntent.Type)
            {
                case "text/plain":
                    {
                        shareSubject = Intent.GetStringExtra(Intent.ExtraSubject);
                        shareDescription = Intent.GetStringExtra(Intent.ExtraText);
                    }; break;
                case "image/*":
                    {

                    }; break;
            }
        }*/

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

