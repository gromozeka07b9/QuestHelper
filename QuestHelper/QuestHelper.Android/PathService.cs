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
using QuestHelper.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(PathService))]
namespace QuestHelper.Droid
{
    public class PathService : IPathService
    {
        public string InternalFolder
        {
            get
            {
                return Android.App.Application.Context.FilesDir.AbsolutePath;
            }
        }

        public string PublicExternalFolder
        {
            get
            {
                return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            }
        }

        public string PrivateExternalFolder
        {
            get
            {
                string absPath = string.Empty;
                try
                {
                    //ToDo: Что это?
                    absPath = Application.Context.GetExternalFilesDir(null).AbsolutePath;
                }
                catch (Exception)
                {
                }
                return absPath;
            }
        }
        public string PublicDirectoryPictures
        {
            get
            {
                return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
            }
        }
        public string PublicDirectoryDcim
        {
            get
            {
                return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).AbsolutePath;
            }
        }
    }
}