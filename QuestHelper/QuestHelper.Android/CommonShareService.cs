using System;
using System.Collections.Generic;
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
                Intent share = new Intent(Intent.ActionSendMultiple);
                //Intent share = new Intent(Intent.ActionSend);
                share.SetFlags(ActivityFlags.NewTask);
                //share.SetAction();
                //share.SetType("text/plain");
                share.SetType("image/*");
                share.PutExtra(Intent.ExtraText, vpoint.Description);
                share.PutExtra(Intent.ExtraSubject, vpoint.Description);
                share.PutExtra(Intent.ExtraSplitName, vpoint.Description);
                share.PutExtra(Intent.ExtraPackageName, vpoint.Description);
                share.PutExtra(Intent.ExtraHtmlText, vpoint.Description);
                //share.PutExtra(Intent.ExtraAllowMultiple, true);

                List<Uri> uris = new List<Uri>();
                foreach (var path in vpoint.MediaObjectPaths)
                {
                    Java.IO.File file = new Java.IO.File(path);
                    var fileUri = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
                    uris.Add(fileUri);
                    //share.PutExtra(Intent.ExtraStream, fileUri);
                }

                share.PutParcelableArrayListExtra(Intent.ExtraStream, uris.ToArray());

                /*share.SetType("image/*");
                File file = new File(filePath);
                Uri uri = Uri.FromFile(file);*/
                /*IList<IParcelable> uris = new List<IParcelable>();
                uris.Add(uri);
                uris.Add(uri);
                share.PutParcelableArrayListExtra(Intent.ExtraStream, uris);*/
                //share.PutExtra(Intent.ExtraStream, uri);
                //Android.App.Application.Context.StartActivity(Intent.CreateChooser(share, "Share..."));
                Android.App.Application.Context.StartActivity(share);
            }
        }
    }
}