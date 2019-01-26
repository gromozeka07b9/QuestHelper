using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestHelper.Droid;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.View;
using QuestHelper.View.Geo;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace QuestHelper.Droid
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        private CustomMap customMap;

        public CustomMapRenderer(Context context) : base(context)
        {
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var point = customMap.Points
                    .Where(x => x.Latitude == marker.Position.Latitude && x.Longitude == marker.Position.Longitude)
                    .FirstOrDefault();

                if (point == null)
                {
                    throw new Exception("Custom pin not found");
                }

                view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);

                var infoImg = view.FindViewById<ImageView>(Resource.Id.InfoWindowImage);
                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoDescription = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoImg != null)
                {
                    Bitmap bm = BitmapFactory.DecodeFile(point.PathToPicture);
                    if (bm != null)
                    {
                        infoImg.SetImageBitmap(cropCenter(bm, 500, 400));
                        bm = null;
                    }
                }
                if (infoTitle != null)
                {
                    infoTitle.Text = point.Name;
                }
                if (infoDescription != null)
                {
                    int maxLength = 100;
                    string text = string.Empty;
                    if (string.IsNullOrEmpty(point.Description))
                    {
                        text = "Описание не заполнено";
                    } else if (point.Description.Length > maxLength)
                    {
                        text = point.Description.Substring(0, maxLength) + "...";
                    } else
                    {
                        text = point.Description;
                    }
                    infoDescription.Text = text;
                }
                return view;
            }
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe
            }

            if (e.NewElement != null)
            {
                customMap = (CustomMap) e.NewElement;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (this.Element == null || this.Control == null)
                return;

            if (e.PropertyName == "")
            {

            }
        }

        protected override void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            base.OnMapReady(map);
            map.MapClick += Map_MapClick;
            foreach (var point in customMap.Points)
            {
                var latlng = new LatLng(point.Latitude, point.Longitude);
                var marker = new MarkerOptions();
                marker.SetPosition(latlng);
                BitmapDescriptor pic = null;
                if (!string.IsNullOrEmpty(point.PathToPicture))
                {
                    Bitmap bm = BitmapFactory.DecodeFile(point.PathToPicture);
                    if (bm != null)
                    {
                        pic = BitmapDescriptorFactory.FromBitmap(cropCenter(bm));
                        bm = null;
                    }
                }

                if (pic == null)
                {
                    pic = BitmapDescriptorFactory.FromResource(Resource.Drawable.place_unknown);
                }
                marker.SetIcon(pic);
                marker.SetTitle("test");
                NativeMap.AddMarker(marker);
            }
            NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);
        }

        public static Bitmap cropCenter(Bitmap bmp, int width = 256, int height = 192)
        {
            bool portrait = bmp.Width < bmp.Height;
            if (portrait)
            {
                int x = width;
                width = height;
                height = x;
            }

            Bitmap result = null;
            try
            {
                result = ThumbnailUtils.ExtractThumbnail(bmp, width, height);
            }
            catch (Exception)
            {

            }
            return result;
        }
        private void Map_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            Xamarin.Forms.MessagingCenter.Send<MapSelectNewPointMessage>(new MapSelectNewPointMessage(){Latitude = e.Point.Latitude , Longitude = e.Point.Longitude }, string.Empty);
            //NativeMap.AddMarker(new MarkerOptions().SetPosition(new LatLng(e.Point.Latitude, e.Point.Longitude)).SetTitle("touch marker"));
        }

        private void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            Xamarin.Forms.MessagingCenter.Send<MapOpenPointMessage>(new MapOpenPointMessage() { Latitude = e.Marker.Position.Latitude, Longitude = e.Marker.Position.Longitude }, string.Empty);
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            Parent.Parent.Parent.Parent.RequestDisallowInterceptTouchEvent(true);
            return base.DispatchTouchEvent(e);
        }

    }
}