using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using QuestHelper.Consts;
using QuestHelper.Droid;
using QuestHelper.Droid.Renderers;
using QuestHelper.Managers;
using QuestHelper.Resources;
using QuestHelper.View;
using QuestHelper.View.Geo;
using System;
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
        private CustomOverviewMap _map;
        //private static int _sizeMarkerDivider = 3;//примерный делитель для получения более менее видимого маркера
        //private int _sizeMarker = Convert.ToInt32(DeviceSize.FullScreenHeight / _sizeMarkerDivider);
        private float _currentZoomLevel;

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
                _map = (CustomOverviewMap)e.NewElement;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        private void NativeMap_CameraIdle(object sender, EventArgs e)
        {
            var map = (Android.Gms.Maps.GoogleMap)sender;
            _currentZoomLevel = map.CameraPosition.Zoom;
        }

        protected override async void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            base.OnMapReady(map);
            NativeMap.CameraIdle += NativeMap_CameraIdle;
        }

        private void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            return MarkerMaker.MakeMarkerByPOI(pin, _currentZoomLevel);
        }
    }
}