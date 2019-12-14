using QuestHelper.iOS;
using System;
using Path = System.IO.Path;

[assembly: Xamarin.Forms.Dependency(typeof(DefaultViewer))]
namespace QuestHelper.iOS
{
    public class DefaultViewer : IDefaultViewer
    {
        public void Show(string filename)
        {
            /*Intent intent = new Intent();
            intent.SetAction(Intent.ActionView);
            intent.AddFlags(ActivityFlags.NewTask);
            intent.AddFlags(ActivityFlags.MultipleTask);
            intent.AddFlags(ActivityFlags.GrantReadUriPermission);
            try
            {
                Java.IO.File file = new Java.IO.File(filename);
                var fileUri = FileProvider.GetUriForFile(Android.App.Application.Context, Android.App.Application.Context.PackageName + ".fileprovider", file);
                if(Path.GetExtension(filename) == ".3gp")
                    intent.SetDataAndType(fileUri, "audio/*");
                else
                    intent.SetDataAndType(fileUri, "image/*");
                Android.App.Application.Context.StartActivity(intent);
            }
            catch (Exception e)
            {
                HandleError.Process("DefaultViewer", "Show", e, false);
            }*/
        }
    }
}