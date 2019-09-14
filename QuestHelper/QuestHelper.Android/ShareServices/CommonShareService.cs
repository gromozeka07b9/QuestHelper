using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.IO;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Droid.ShareServices;
using QuestHelper.Model;
using Uri = Android.Net.Uri;

[assembly: Xamarin.Forms.Dependency(typeof(CommonShareService))]
namespace QuestHelper.Droid.ShareServices
{
    public class CommonShareService : ICommonShareService
    {
        public void Share(ViewRoutePoint vpoint, string packageName)
        {
            if ((vpoint != null) && (!string.IsNullOrEmpty(vpoint.Id)))
            {
                string pointCoordinates = $"{vpoint.Latitude.ToString(CultureInfo.InvariantCulture)},{vpoint.Longitude.ToString(CultureInfo.InvariantCulture)}";
                Intent share = new Intent();
                share.SetType("text/plain");
                if (vpoint.MediaObjectPaths.Count > 1)
                {
                    share = new Intent(Intent.ActionSendMultiple);
                    share.SetType("image/*");
                    List<Uri> uris = new List<Uri>();
                    foreach (var path in vpoint.MediaObjectPaths)
                    {
                        Java.IO.File file = new Java.IO.File(path);
                        var fileUri = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
                        uris.Add(fileUri);
                    }
                    share.PutParcelableArrayListExtra(Intent.ExtraStream, uris.ToArray());
                }
                else if (vpoint.MediaObjectPaths.Count == 1)
                {
                    share = new Intent(Intent.ActionSend);
                    share.SetType("image/*");
                    Java.IO.File file = new Java.IO.File(vpoint.MediaObjectPaths[0]);
                    var fileUri = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
                    share.PutExtra(Intent.ExtraStream, fileUri);
                } else if (vpoint.MediaObjectPaths.Count == 0)
                {

                }
                share.SetFlags(ActivityFlags.NewTask);
                share.PutExtra(Intent.ExtraText, $"{pointCoordinates}\n{vpoint.Description}");
                share.PutExtra(Intent.ExtraSubject, $"{pointCoordinates}\n{vpoint.NameText}");
                share.PutExtra(Intent.ExtraAllowMultiple, true);

                if (!string.IsNullOrEmpty(packageName))
                {
                    AddComponentNameToIntent(packageName, share);
                }

                try
                {
                    Analytics.TrackEvent("Common share service");
                    Android.App.Application.Context.StartActivity(share);
                }
                catch (Exception e)
                {
                    HandleError.Process("CommonShareService", "Share point", e, false);
                }
            }
        }

        public void Share(ViewRoute vroute, string packageName)
        {

            Intent share = new Intent(Intent.ActionSend);
            share.SetType("image/*");

            if (!string.IsNullOrEmpty(packageName))
            {
                AddComponentNameToIntent(packageName, share);
            }

            Java.IO.File file = new Java.IO.File(vroute.ImagePreviewPathForList);
            var fileUri = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
            share.PutExtra(Intent.ExtraStream, fileUri);

            share.SetFlags(ActivityFlags.NewTask);
            share.PutExtra(Intent.ExtraText, $"{vroute.Name}");
            share.PutExtra(Intent.ExtraSubject, $"{vroute.CreateDateText}");
            share.PutExtra(Intent.ExtraAllowMultiple, true);
            try
            {
                Analytics.TrackEvent("Common share service");
                Android.App.Application.Context.StartActivity(share);
            }
            catch (Exception e)
            {
                HandleError.Process("CommonShareService", "Share route", e, false);
            }
        }

        public void AddComponentNameToIntent(string packageName, Intent share)
        {
            var activities = Android.App.Application.Context.PackageManager.QueryIntentActivities(share, 0);
            var appActivity = activities.FirstOrDefault(a => a.ActivityInfo.PackageName == packageName);
            if (appActivity != null)
            {
                var componentName = new ComponentName(appActivity.ActivityInfo.PackageName, appActivity.ActivityInfo.Name);
                share.SetComponent(componentName);
            }
        }
    }
}