using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
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
        private static MainActivity _activity;

        public static BottomBar Bar
        {
            get { return _bottomBar; }
        }
        public static void CreateToolbar(MainActivity activity, Bundle bundle)
        {
            _activity = activity;
            _bottomBar = BottomBar.Attach(activity, bundle);
            //_bottomBar.UseFixedMode();
            //_bottomBar.SetFixedInactiveIconColor("#000000");
            _bottomBar.SetActiveTabColor("#3A3A9C");
            _bottomBar.SetItems(Resource.Menu.bottombar_menu);
            _bottomBar.SetOnTabClickListener(activity);
        }
        public void SetVisibilityToolbar(bool Visibility)
        {
            if (Visibility)
            {
                //_bottomBar.Show(false);
                var layout = _activity.FindViewById<LinearLayout>(Resource.Id.bb_bottom_bar_item_container);
                layout.Visibility = ViewStates.Visible;
                _bottomBar.Invalidate();
                    //.RefreshDrawableState();
            }
            else
            {
                //_bottomBar.Hide(false);
                var layout = _activity.FindViewById<LinearLayout>(Resource.Id.bb_bottom_bar_item_container);
                layout.Visibility = ViewStates.Gone;
            }
        }
        public void SetDarkMode(bool DarkMode)
        {
            if (!DarkMode)
            {
                //Корректно не работает скрытие, оставляю пока видимой панель, но со сменой цвета для альбома
                //проблема в том, что при скрытии панели наверху остается поле таба, наверное надо копать в сторону tabbar.axml и менять там размер
                _bottomBar.SetItems(Resource.Menu.bottombar_menu);
                var container = _bottomBar.ItemContainer;
                container.SetBackgroundColor(Color.White);
            }
            else
            {
                var container = _bottomBar.ItemContainer;
                container.SetBackgroundColor(Color.Black);
            }
        }

        public bool ToolbarIsHidden()
        {
            return _bottomBar.Hidden;
        }

    }
}