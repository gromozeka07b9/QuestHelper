using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Accounts;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.AppCenter.Crashes;
using Plugin.CurrentActivity;
using QuestHelper.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.Dependency(typeof(DefaultViewer))]
namespace QuestHelper.Droid
{
    public class DefaultViewer : IDefaultViewer
    {
        public void Show(string filename)
        {
            Intent intent = new Intent();
            intent.SetAction(Intent.ActionView);
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.MultipleTask);
            try
            {
                intent.SetDataAndType(Android.Net.Uri.Parse("file:///" + filename), "image/*");
                Android.App.Application.Context.StartActivity(intent);
            }
            catch (Exception e)
            {
                HandleError.Process("DefaultViewer", "Show", e, false);
            }
        }
    }
}