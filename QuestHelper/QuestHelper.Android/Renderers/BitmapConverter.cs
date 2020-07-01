using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace QuestHelper.Droid.Renderers
{
    internal class BitmapConverter
    {
        static Bitmap bitmapBackCircleSrc = BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.markerback7);

        public static Bitmap Crop(Bitmap bmp, int radius)
        {
            return getCircleBitmap(bmp, radius);
        }

        private static Bitmap getCircleBitmap(Bitmap bmp, int radius)
        {
            Bitmap sbmp;
            if (bmp.Width != radius || bmp.Height != radius)
                sbmp = Bitmap.CreateScaledBitmap(bmp, radius - 10, radius - 10, false);
            else
                sbmp = bmp;

            Bitmap output = Bitmap.CreateBitmap(sbmp.Width, sbmp.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(output);
            Paint paint = new Paint();
            paint.FilterBitmap = true;
            canvas.DrawCircle(sbmp.Width / 2 + 0.0f, sbmp.Height / 2 + 0.0f, sbmp.Width / 2 + 0.0f, paint);
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            try
            {
                Rect rect = new Rect(0, 0, sbmp.Width, sbmp.Height);
                canvas.DrawBitmap(sbmp, rect, rect, paint);
            }
            catch (System.Exception ex)
            {
            }

            return output;
        }
    }
}