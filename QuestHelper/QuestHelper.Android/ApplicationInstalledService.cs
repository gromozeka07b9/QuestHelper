using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(ApplicationInstalledService))]
namespace QuestHelper.Droid
{
    public class ApplicationInstalledService : IApplicationInstalledService
    {
        public bool AppInstalled(string appName)
        {
            bool result = false;
            try
            {
                var appInfo = Android.App.Application.Context.PackageManager.GetApplicationInfo(appName, 0);
                result = appInfo.Enabled;
            }
            catch (Exception)
            {

            }

            return result;
        }
    }
}