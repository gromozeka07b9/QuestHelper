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
            base.Share(vroute, packageName);
        }
    }
}