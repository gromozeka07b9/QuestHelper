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
using Acr.UserDialogs;
using Android.Content;
using Android.Support.V7.App;
using AlertDialog = Android.App.AlertDialog;
using QuestHelper.Model.Messages;
using Microsoft.AppCenter.Push;

namespace QuestHelper.Droid
{
    [IntentFilter(new[] { Intent.ActionView, Intent.ActionEdit, Intent.ActionSend, Intent.ActionMain }, Label = "Gosh!", Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataMimeType = "text/plain")]
    [Activity(Label = "Gosh!", Icon = "@drawable/icon", Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true, LaunchMode = LaunchMode.SingleInstance, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        private string shareSubject = string.Empty;
        private string shareDescription = string.Empty;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            UserDialogs.Init(this);
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

        protected override void OnNewIntent(Android.Content.Intent intent)
        {
            base.OnNewIntent(intent);
            Push.CheckLaunchedFromNotification(this, intent);
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

