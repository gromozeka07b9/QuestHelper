﻿using System;
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
        static Bitmap bitmapBackCircleSrc = BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.markerback5);
        static Bitmap bitmapBackRectangleSrc3 = BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.markerback3);
        static Bitmap bitmapBackRectangleSrc4 = BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.markerback4);

        public static Bitmap Crop(Bitmap bmp, int radius)
        {
            //return getRectangleBitmap4(bmp, radius);
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
            paint.AntiAlias = true;
            paint.FilterBitmap = true;
            paint.Dither = true;
            canvas.DrawARGB(0, 0, 0, 0);
            paint.Color = Android.Graphics.Color.ParseColor("#000000");

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
            Paint paint1 = new Paint();
            paint.AntiAlias = true;
            paint.FilterBitmap = true;
            paint.Dither = true;
            //Bitmap bitmapBackSrc = BitmapFactory.DecodeResource(Android.App.Application.Context.Resources, Resource.Drawable.markerback2);
            var bitmapBackCircle = bitmapBackCircleSrc.Copy(Bitmap.Config.Argb8888, true);
            Canvas canvas1 = new Canvas(bitmapBackCircle);
            Rect rectMin = new Rect(0, 0, bmp.Width, bmp.Height);
            int margin = 0;
            int marginBottom = Convert.ToInt32(bitmapBackCircle.Height * 0.14);
            //Rect rectMax = new Rect(margin, margin, bitmapBack.Width - margin, bitmapBack.Height - (margin + marginBottom));
            //canvas.DrawCircle(bitmapBack.Width / 2 + 0.0f, bitmapBack.Height / 2 - marginBottom + 0.0f, bitmapBack.Width / 2 - margin + 0.0f, paint1);
            //paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            Rect rectMax = new Rect(10, 10, 290, 290);
            canvas1.DrawBitmap(output, null, rectMax, paint1);
            return bitmapBackCircle;
        }

        private static Bitmap getRectangleBitmap3(Bitmap bmp, int radius)
        {
            var bitmapBack = bitmapBackRectangleSrc3.Copy(Bitmap.Config.Argb8888, true);
            Canvas canvas = new Canvas(bitmapBack);
            Rect rectMin = new Rect(0, 0, bmp.Width, bmp.Height);
            int margin = 10;
            int marginBottom = Convert.ToInt32(bitmapBack.Height * 0.175);
            Rect rectMax = new Rect(margin, margin, bitmapBack.Width - margin, bitmapBack.Height - (margin + marginBottom));
            canvas.DrawBitmap(bmp, rectMin, rectMax, null);
            return bitmapBack;
        }
        private static Bitmap getRectangleBitmap4(Bitmap bmp, int radius)
        {
            var bitmapBack = bitmapBackRectangleSrc4.Copy(Bitmap.Config.Argb8888, true);
            Canvas canvas = new Canvas(bitmapBack);
            int marginLeft = 10;
            int marginRight = 18;
            int marginTop = Convert.ToInt32(bitmapBack.Height * 0.25);
            int marginBottom = Convert.ToInt32(bitmapBack.Height * 0.05);
            int marginSrc = 0;
            if (bmp.Width <= bmp.Height) marginSrc = bmp.Height - bmp.Width;
            Rect rectMin = new Rect(marginSrc, marginSrc + marginSrc /2, bmp.Width - marginSrc, bmp.Height - marginSrc/2);
            Rect rectMax = new Rect(marginLeft, marginTop, bitmapBack.Width - marginRight, bitmapBack.Height - marginBottom);
            canvas.DrawBitmap(bmp, rectMin, rectMax, null);

            //var textPaint = new Paint(PaintFlags.AntiAlias);
            //textPaint.TextSize = 30;
            //canvas.DrawText("Название", 27, 60, textPaint);
            return bitmapBack;
        }
    }
}