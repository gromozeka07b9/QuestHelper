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

namespace QuestHelper.Droid
{
    [Activity(Label = "", Theme = "@style/MainTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(()=> { SimulateStartup(); });
            startupWork.Start();
        }

        async void SimulateStartup()
        {
            await Task.Delay(1000);
            StartActivity(new Intent(this, typeof(MainActivity)));
        }
    }
}

