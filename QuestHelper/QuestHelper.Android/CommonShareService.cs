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
using QuestHelper.Droid;
using QuestHelper.Model;
using Uri = Android.Net.Uri;

[assembly: Xamarin.Forms.Dependency(typeof(CommonShareService))]
namespace QuestHelper.Droid
{
    public class CommonShareService : ICommonShareService
    {
        public void Share(ViewRoutePoint vpoint)
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

                try
                {
                    Analytics.TrackEvent("Common share service");
                    Android.App.Application.Context.StartActivity(share);
                }
                catch (Exception e)
                {
                    HandleError.Process("CommonShareService", "Share", e, false);
                }
            }
        }
    }
}