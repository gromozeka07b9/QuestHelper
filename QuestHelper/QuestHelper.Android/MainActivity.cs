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

namespace QuestHelper.Droid
{
    [Activity(Label = "QuestHelper", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, BottomNavigationBar.Listeners.IOnTabClickListener
    {
        private BottomBar _bottomBar;

        public void OnTabReSelected(int position)
        {
        }

        public void OnTabSelected(int position)
        {
            var pageCollections = new PagesCollection();
            MainPageMenuItem destinationPage = pageCollections.GetPageByPosition(position);
            Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(new PageNavigationMessage() { DestinationPageDescription = destinationPage}, string.Empty);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            //_bottomBar.MapColorForTab(0, ContextCompat.GetColor(this, Resource.Color.colorAccent));
            //_bottomBar.MapColorForTab(1, "#FF5D4037");
            //_bottomBar.MapColorForTab(2, "#7B1FA2");
            //_bottomBar.MapColorForTab(3, "#FF5252");
            //_bottomBar.MapColorForTab(4, "#FF9800");
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            LoadApplication(new App());
            _bottomBar = BottomBar.Attach(this, bundle);
            _bottomBar.UseFixedMode();
            _bottomBar.SetFixedInactiveIconColor("#888888");
            _bottomBar.SetActiveTabColor("#ffffff");
            _bottomBar.SetItems(Resource.Menu.bottombar_menu);
            _bottomBar.MapColorForTab(1, "#FF9800");
            _bottomBar.SetOnTabClickListener(this);
            _bottomBar.UseDarkTheme();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            _bottomBar.OnSaveInstanceState(outState);
        }
    }
}

