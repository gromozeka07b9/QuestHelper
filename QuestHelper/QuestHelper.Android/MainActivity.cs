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
            AuthService authService = new AuthService();
            var pageCollections = new PagesCollection();
            MainPageMenuItem destinationPage = pageCollections.GetPageByPosition(position);
            if (string.IsNullOrEmpty(authService.GetAuthToken()))
            {
                destinationPage = pageCollections.GetLoginPage();
            }
            Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(new PageNavigationMessage() { DestinationPageDescription = destinationPage}, string.Empty);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            
            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            LoadApplication(new App());
            _bottomBar = BottomBar.Attach(this, bundle);
            _bottomBar.UseFixedMode();
            _bottomBar.SetFixedInactiveIconColor("#B3B8C2");
            _bottomBar.SetActiveTabColor("#3A3A9C");
            AuthService authService = new AuthService();
            /*if (string.IsNullOrEmpty(authService.GetAuthToken()))
            {
                _bottomBar.Visibility = ViewStates.Invisible;
            }
            else
            {
                _bottomBar.Visibility = ViewStates.Visible;
            }*/
            _bottomBar.SetItems(Resource.Menu.bottombar_menu);
            _bottomBar.SetOnTabClickListener(this);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            _bottomBar.OnSaveInstanceState(outState);
        }
    }
}

