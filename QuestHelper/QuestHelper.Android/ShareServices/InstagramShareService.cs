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

[assembly: Xamarin.Forms.Dependency(typeof(InstagramShareService))]
namespace QuestHelper.Droid.ShareServices
{
    public class InstagramShareService : CommonShareService, IInstagramShareService
    {
        public new void Share(ViewRoutePoint vpoint, string packageName)
        {
            base.Share(vpoint, packageName);
        }

        public new void Share(ViewRoute vroute, string packageName)
        {
            if ((vroute != null) && (!string.IsNullOrEmpty(vroute.Id)))
            {
                RoutePointManager pointManager = new RoutePointManager();
                var routePoints = pointManager.GetPointsByRouteId(vroute.RouteId);
                Intent share = new Intent(Intent.ActionSend);
                share.SetType("image/*");
                if (routePoints.Any())
                {
                    Java.IO.File file = new Java.IO.File(vroute.ImagePreviewPathForList);
                    var fileUri = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
                    share.PutExtra(Intent.ExtraStream, fileUri);
                    share.AddFlags(ActivityFlags.GrantReadUriPermission);
                }

                var componentName = new ComponentName("com.instagram.android", "com.instagram.share.handleractivity.ShareHandlerActivity");
                share.SetComponent(componentName);

                try
                {
                    var intentNew = Intent.CreateChooser(share, "Share to");
                    intentNew.SetFlags(ActivityFlags.NewTask);
                    Android.App.Application.Context.StartActivity(intentNew);
                }
                catch (Exception e)
                {
                    HandleError.Process("Instagram", "Share route", e, false);
                }
            }
        }
    }
}