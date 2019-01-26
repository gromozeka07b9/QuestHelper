﻿using System;

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
using QuestHelper.Model.Messages;
using Plugin.CurrentActivity;
using ImageCircle.Forms.Plugin.Droid;

namespace QuestHelper.Droid
{
    [Activity(Label = "QuestHelper", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, BottomNavigationBar.Listeners.IOnTabClickListener
    {

        public void OnTabReSelected(int position)
        {
            NavigateToPage(position);
        }

        public void OnTabSelected(int position)
        {
            NavigateToPage(position);
        }

        private static void NavigateToPage(int position)
        {
            var pageCollections = new PagesCollection();
            MainPageMenuItem destinationPage = pageCollections.GetPageByPosition(position);
            Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(
                new PageNavigationMessage() {DestinationPageDescription = destinationPage}, string.Empty);
        }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            CrossCurrentActivity.Current.Init(this, bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarin.FormsMaps.Init(this, bundle);
            ImageCircleRenderer.Init();
            LoadApplication(new App());

            StartMainScreen(bundle);
        }

        private void StartMainScreen(Bundle bundle)
        {
            ToolbarService.CreateToolbar(this, bundle);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (ToolbarService.Bar != null)
            {
                ToolbarService.Bar.OnSaveInstanceState(outState);
            }
        }
    }
}

