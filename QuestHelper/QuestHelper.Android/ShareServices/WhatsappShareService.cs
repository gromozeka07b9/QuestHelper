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

[assembly: Xamarin.Forms.Dependency(typeof(WhatsappShareService))]
namespace QuestHelper.Droid.ShareServices
{
    public class WhatsappShareService : CommonShareService, IWhatsappShareService
    {
        public void Share(ViewRoutePoint vpoint, string packageName)
        {
            base.Share(vpoint, packageName);
        }

        public void Share(ViewRoute vroute, string packageName)
        {
        //https://stackoverflow.com/questions/30196530/share-image-plain-text-and-html-text-via-intent
        //https://guides.codepath.com/android/Sharing-Content-with-Intents
            if ((vroute != null) && (!string.IsNullOrEmpty(vroute.Id)))
            {
                /*waIntent.putExtra(Intent.EXTRA_TEXT, text);
                waIntent.putExtra(Intent.EXTRA_SUBJECT, "My subject");
                waIntent.putExtra(Intent.EXTRA_TITLE, "My subject");
                waIntent.putExtra(Intent.EXTRA_TEXT, "https://play.google.com/store/apps/details?id=" + BuildConfig.APPLICATION_ID);
                waIntent.putExtra(Intent.EXTRA_STREAM, attachment);*/
            }
        }
    }
}