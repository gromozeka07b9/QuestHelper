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
using QuestHelper.Managers;
using QuestHelper.Model;
using Uri = Android.Net.Uri;

[assembly: Xamarin.Forms.Dependency(typeof(FacebookShareService))]
namespace QuestHelper.Droid.ShareServices
{
    public class FacebookShareService : CommonShareService, IFacebookShareService
    {
        public void Share(ViewRoutePoint vpoint, string packageName)
        {
            base.Share(vpoint, packageName);
        }

        public void Share(ViewRoute vroute, string packageName)
        {
            if ((vroute != null) && (!string.IsNullOrEmpty(vroute.Id)))
            {
                RoutePointManager pointManager = new RoutePointManager();
                var routePoints = pointManager.GetPointsByRouteId(vroute.RouteId);
                Intent share = new Intent(Intent.ActionSendMultiple);
                share.SetType("image/*");
                List<Uri> uris = new List<Uri>();
                if (routePoints.Any())
                {
                    Java.IO.File file = new Java.IO.File(vroute.ImagePreviewPathForList);
                    var fileUri = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
                    share.PutExtra(Intent.ExtraStream, fileUri);
                    uris.Add(fileUri);
                    share.PutParcelableArrayListExtra(Intent.ExtraStream, uris.ToArray());
                }
                share.SetFlags(ActivityFlags.NewTask);

                if (!string.IsNullOrEmpty(packageName))
                {
                    AddComponentNameToIntent(packageName, share);
                }

                try
                {
                    Analytics.TrackEvent("Facebook share service");
                    Android.App.Application.Context.StartActivity(share);
                }
                catch (Exception e)
                {
                    HandleError.Process("Facebook", "Share route", e, false);
                }
            }
        }
    }
}