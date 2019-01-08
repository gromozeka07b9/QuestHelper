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
using Java.IO;
using QuestHelper.Droid;
using Uri = Android.Net.Uri;

[assembly: Xamarin.Forms.Dependency(typeof(InstagramShareService))]
namespace QuestHelper.Droid
{
    public class InstagramShareService : IInstagramShareService
    {
        public void Share(string filePath)
        {
            Intent share = new Intent(Intent.ActionSend);
            share.SetType("image/*");
            File file = new File(filePath);
            Uri uri = Uri.FromFile(file);
            /*IList<IParcelable> uris = new List<IParcelable>();
            uris.Add(uri);
            uris.Add(uri);
            share.PutParcelableArrayListExtra(Intent.ExtraStream, uris);*/
            share.PutExtra(Intent.ExtraStream, uri);
            Android.App.Application.Context.StartActivity(share);
        }
    }
}