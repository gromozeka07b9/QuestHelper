using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using QuestHelper.Droid;
using QuestHelper.Droid.Renderers;
using QuestHelper.Managers;
using QuestHelper.Resources;
using QuestHelper.View;
using QuestHelper.View.Geo;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomOverviewMap), typeof(CustomOverviewMapRenderer))]
namespace QuestHelper.Droid.Renderers
{
    public class CustomOverviewMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        private CustomOverviewMap map;


        public CustomOverviewMapRenderer(Context context) : base(context)
        {
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
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
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

        }

        protected override async void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            PermissionManager permissions = new PermissionManager();
            if (await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Location, CommonResource.Permission_Position))
            {
            }
            base.OnMapReady(map);
        }

        private void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
        }

        private void drawMarkers(int markerSize)
        {
            foreach(MarkerOptions marker in MarkerMaker.MakeMarkersByPOI(map.POIs))
            {
                NativeMap.AddMarker(marker);
            }


            /*LatLngBounds.Builder bounds = new LatLngBounds.Builder();
            NativeMap.Clear();
            List<LatLng> latLngList = new List<LatLng>();
            foreach (var pin in map.POIs)
            {
                var latlng = new LatLng(pin.Position.Latitude, pin.Position.Longitude);
                latLngList.Add(latlng);
                var marker = new MarkerOptions();
                marker.Anchor(0.5f, 0.5f);
                marker.SetPosition(latlng);
                BitmapDescriptor pic = null;
                if (!string.IsNullOrEmpty(point.PathToPicture))
                {
                    Bitmap bm = BitmapFactory.DecodeFile(pin..PathToPicture);
                    if (bm != null)
                    {
                        var croppedBitmap = getCroppedBitmap(bm, markerSize);
                        pic = BitmapDescriptorFactory.FromBitmap(croppedBitmap);
                        bm = null;
                        croppedBitmap = null;
                    }
                }

                if (pic == null)
                {
                    pic = BitmapDescriptorFactory.FromResource(Resource.Drawable.place_unknown);
                }

                marker.SetIcon(pic);
                NativeMap.AddMarker(marker);
                bounds.Include(latlng);
            }
            List<PatternItem> pattern_lines = new List<PatternItem>();
            pattern_lines.Add(new Gap(20));
            pattern_lines.Add(new Dash(20));
            PolylineOptions lineOptions = new PolylineOptions();
            foreach (var point in latLngList)
            {
                lineOptions.Add(point);
            }
            lineOptions.InvokePattern(pattern_lines);
            lineOptions.InvokeWidth(10);
            NativeMap.AddPolyline(lineOptions);
            if (customMap.Points.Count > 1)
            {
                try
                {
                    NativeMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds.Build(), 100));
                }
                catch (Java.Lang.Exception e)
                {

                }
            }
            bounds.Dispose();*/
        }
    }
}