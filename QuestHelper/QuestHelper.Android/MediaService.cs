using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(MediaService))]
namespace QuestHelper.Droid
{
    public class MediaService : IMediaService
    {
        public byte[] ResizeImage(byte[] imageData, float width, float height, int quality)
        {
            // Load the bitmap 
            BitmapFactory.Options options = new BitmapFactory.Options();// Create object of bitmapfactory's option method for further option use
            options.InPurgeable = true; // inPurgeable is used to free up memory while required
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);

            float newHeight = 0;
            float newWidth = 0;

            var originalHeight = originalImage.Height;
            var originalWidth = originalImage.Width;

            if ((width > 0) && (height > 0))
            {

                if (originalHeight > originalWidth)
                {
                    newHeight = height;
                    float ratio = originalHeight / height;
                    newWidth = originalWidth / ratio;
                }
                else
                {
                    newWidth = width;
                    float ratio = originalWidth / width;
                    newHeight = originalHeight / ratio;
                }
            }
            else
            {
                newWidth = originalWidth;
                newHeight = originalHeight;
            }

            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, true);

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
                resizedImage.Recycle();
                originalImage.Recycle();
                return ms.ToArray();
            }
        }
    }
}