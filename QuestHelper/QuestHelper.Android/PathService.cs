using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Java.IO;
using QuestHelper.Droid;
using Console = System.Console;

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
                return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures)?.AbsolutePath;
            }
        }
        public string PublicDirectoryDcim
        {
            get
            {
                return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim)?.AbsolutePath;
            }
        }
        public string GetLastUsedDCIMPath()
        {
                string[] projection = new[]
                {
                    MediaStore.Images.ImageColumns.Id, 
                    MediaStore.Images.ImageColumns.Data,
                    MediaStore.Images.ImageColumns.BucketDisplayName,
                    MediaStore.Images.ImageColumns.DateTaken,
                    MediaStore.Images.ImageColumns.MimeType
                };
                ICursor cursor = null;
                try
                {
                    cursor = Application.Context.ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri,
                        projection,
                        null,
                        null,
                        MediaStore.Images.ImageColumns.DateTaken + " DESC"
                    );
                    if ((cursor != null) && (cursor.Count > 0))
                    {
                        cursor.MoveToFirst();
                        do
                        {
                            string path = cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data));
                            if (path.Contains("/DCIM/"))
                            {
                                File file = new File(path);
                                path = file.Parent;
                                cursor.Close();
                                return path;
                            }
                        } while (cursor.MoveToNext());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    cursor?.Close();
                }

                return string.Empty;
        }
    }
}