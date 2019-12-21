using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using QuestHelper;
using Xamarin.Forms;
using QuestHelper.Model;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Android.Content;
using Android.Support.V7.App;
using AlertDialog = Android.App.AlertDialog;
using QuestHelper.Model.Messages;
using Android.Gms.Common;
using System.Globalization;
using System.Threading;
using Android.Preferences;
using Java.Util;
using Android.Content.Res;

namespace QuestHelper.Droid
{
    [IntentFilter(new[] { Intent.ActionView, Intent.ActionEdit, Intent.ActionSend, Intent.ActionMain }, Label = "Gosh!", Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataMimeType = "text/plain")]
#if DEBUG
    [Activity(Label = "Gosh! Debug", Icon = "@drawable/icon2", Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true, LaunchMode = LaunchMode.SingleInstance, ScreenOrientation = ScreenOrientation.Portrait)]
#else
    [Activity(Label = "Gosh!", Icon = "@drawable/icon", Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true, LaunchMode = LaunchMode.SingleInstance, ScreenOrientation = ScreenOrientation.Portrait)]
#endif
    public class SplashActivity : AppCompatActivity
    {
        private string shareSubject = string.Empty;
        private string shareDescription = string.Empty;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            UserDialogs.Init(this);
            if (Intent.Extras != null)
            {
                if (Intent.Extras.KeySet().Count > 0)
                {
                    //ToDo: несмотря на передачу extra в FirebaseNotificationService они почему-то не передаются
                    //Актуально когда гош открываешь из сообщения в шторке
                    string messageBody = Intent.Extras.GetString("messageBodyText");
                    Xamarin.Forms.MessagingCenter.Send<ReceivePushMessage>(new ReceivePushMessage() { MessageBody = messageBody, MessageTitle = string.Empty }, string.Empty);
                }
            }

            if (Intent != null)
            {
                processShareIntent(Intent);
            }

        }

        private void processShareIntent(Intent shareIntent)
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
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(()=> { SimulateStartup(); });
            startupWork.Start();
        }

        async void SimulateStartup()
        {
            await Task.Delay(0);
            Intent mainActivity = new Intent(this, typeof(MainActivity));
            mainActivity.PutExtra("shareSubject", shareSubject);
            mainActivity.PutExtra("shareDescription", shareDescription);
            StartActivity(mainActivity);
        }
    }
}

