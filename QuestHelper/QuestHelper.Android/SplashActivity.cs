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
using Android.Support.V7.App;
using AlertDialog = Android.App.AlertDialog;
using QuestHelper.Model.Messages;

namespace QuestHelper.Droid
{
    [IntentFilter(new[] { Intent.ActionView, Intent.ActionEdit, Intent.ActionSend, Intent.ActionMain }, Label = "Gosh!", Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataMimeType = "text/plain")]
    [Activity(Label = "Gosh!", Icon = "@drawable/icon", Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        private string shareSubject = string.Empty;
        private string shareDescription = string.Empty;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
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
                        //googleMapsMessage.Subject = subject;
                        //googleMapsMessage.Description = description;
                        //Xamarin.Forms.MessagingCenter.Send<ShareFromGoogleMapsMessage>(new ShareFromGoogleMapsMessage() { Subject = subject, Description = description }, "");
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
            //Bundle startBundle = new Bundle();
            //startBundle.PutString("shareSubject", shareSubject);
            //startBundle.PutString("shareDescription", shareDescription);
            Intent mainActivity = new Intent(this, typeof(MainActivity));
            mainActivity.PutExtra("shareSubject", shareSubject);
            mainActivity.PutExtra("shareDescription", shareDescription);
            StartActivity(mainActivity);
        }
    }
}

