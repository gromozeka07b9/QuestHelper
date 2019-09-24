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

[assembly: Xamarin.Forms.Dependency(typeof(TelegramShareService))]
namespace QuestHelper.Droid.ShareServices
{
    public class TelegramShareService : CommonShareService, ITelegramShareService
    {
        /*public void Share(ViewRoutePoint vpoint, string packageName)
        {
            base.Share(vpoint, packageName);
        }*/

        public void ShareRouteOnlyPhotos(ViewRoute vroute, string packageName)
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
                    foreach (var point in routePoints)
                    {
                        foreach (var path in point.MediaObjectPaths)
                        {
                            Java.IO.File file = new Java.IO.File(path);
                            var fileUri = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
                            uris.Add(fileUri);
                        }
                    }
                    share.PutParcelableArrayListExtra(Intent.ExtraStream, uris.ToArray());
                }
                share.PutExtra(Intent.ExtraAllowMultiple, true);
                share.SetFlags(ActivityFlags.NewTask);

                if (!string.IsNullOrEmpty(packageName))
                {
                    AddComponentNameToIntent(packageName, share);
                }

                try
                {
                    Analytics.TrackEvent("Telegram share service photos");
                    Android.App.Application.Context.StartActivity(share);
                }
                catch (Exception e)
                {
                    HandleError.Process("TelegramShareService", "Share route", e, false);
                }

            }

        }
        public void ShareRouteOnlyPointsDescription(ViewRoute vroute, string packageName)
        {
            if ((vroute != null) && (!string.IsNullOrEmpty(vroute.Id)))
            {
                RoutePointManager pointManager = new RoutePointManager();
                var routePoints = pointManager.GetPointsByRouteId(vroute.RouteId);
                Intent share = new Intent(Intent.ActionSend);
                share.SetType("text/*");
                List<Uri> uris = new List<Uri>();
                StringBuilder sbRoute = new StringBuilder();
                sbRoute.AppendLine($"Маршрут: {vroute.Name}");
                sbRoute.AppendLine($"Дата: {vroute.CreateDateText}");
                if (routePoints.Any())
                {
                    foreach (var point in routePoints)
                    {
                        sbRoute.AppendLine("");
                        sbRoute.AppendLine($"Дата: {point.CreateDateText}");
                        sbRoute.AppendLine($"Точка: {point.Name}");
                        if((!point.Latitude.Equals(0d))&&(!point.Longitude.Equals(0d)))
                            sbRoute.AppendLine($"Координаты: {point.Latitude}.{point.Longitude}");
                        if(!string.IsNullOrEmpty(point.Address))
                            sbRoute.AppendLine($"Адрес: {point.Address}");
                        if (!string.IsNullOrEmpty(point.Description))
                            sbRoute.AppendLine($"Описание: {point.Description}");
                    }
                }

                share.PutExtra(Intent.ExtraText, $"{sbRoute.ToString()}");
                share.SetFlags(ActivityFlags.NewTask);

                if (!string.IsNullOrEmpty(packageName))
                {
                    AddComponentNameToIntent(packageName, share);
                }

                try
                {
                    Analytics.TrackEvent("Telegram share service texts");
                    Android.App.Application.Context.StartActivity(share);
                }
                catch (Exception e)
                {
                    HandleError.Process("TelegramShareService", "Share route", e, false);
                }

            }

        }
    }
}