using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BottomNavigationBar;
using QuestHelper.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(ToolbarService))]
namespace QuestHelper.Droid
{
    public class ToolbarService : IToolbarService
    {
        public static BottomBar _bottomBar;

        public static BottomBar Bar
        {
            get { return _bottomBar; }
        }
        public static void CreateToolbar(MainActivity activity, Bundle bundle)
        {
            _bottomBar = BottomBar.Attach(activity, bundle);
            _bottomBar.UseFixedMode();
            _bottomBar.SetFixedInactiveIconColor("#B3B8C2");
            _bottomBar.SetActiveTabColor("#3A3A9C");
            _bottomBar.SetItems(Resource.Menu.bottombar_menu);
            _bottomBar.SetOnTabClickListener(activity);
        }
        public void SetVisibilityToolbar(bool Visibility)
        {
            if (Visibility)
            {
                _bottomBar.Show(false);
                _bottomBar.SetItems(Resource.Menu.bottombar_menu);
            }
            else
            {
                _bottomBar.Hide(false);
            }
        }

        public bool ToolbarIsHidden()
        {
            return _bottomBar.Hidden;
        }

    }
}