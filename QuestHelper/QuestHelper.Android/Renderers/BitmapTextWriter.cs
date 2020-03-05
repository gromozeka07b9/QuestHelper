using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace QuestHelper.Droid.Renderers
{
    public class BitmapTextWriter
    {
        public static Bitmap Write(Bitmap srcBitmap, string text, int left, int top, int textSize)
        {
            var bitmapBack = srcBitmap.Copy(Bitmap.Config.Argb8888, true);
            Canvas canvas = new Canvas(bitmapBack);
            var textPaint = new Paint(PaintFlags.AntiAlias);
            textPaint.TextSize = textSize;//30
            //canvas.DrawText(text, 27, 60, textPaint);
            canvas.DrawText(text, left, top, textPaint);
            return bitmapBack;
        }
    }
}