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
        private static int _sizeMarkerDivider = 3;//примерный делитель для получения более менее видимого маркера
        private int _sizeMarker = Convert.ToInt32(DeviceSize.FullScreenHeight / _sizeMarkerDivider);

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
            drawMarkers(_sizeMarker);
        }

        protected override async void OnMapReady(Android.Gms.Maps.GoogleMap map)
        {
            PermissionManager permissions = new PermissionManager();
            if (await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Location, CommonResource.Permission_Position))
            {
            }
            drawMarkers(_sizeMarker);
            map.MarkerClick += MapOnMarkerClick;
            base.OnMapReady(map);
        }

        void MapOnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs markerClickEventArgs)
        {
            markerClickEventArgs.Handled = true;
            var marker = markerClickEventArgs.Marker;
            
        }

        private void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
        }

        private void drawMarkers(int markerSize)
        {
            foreach(MarkerOptions marker in MarkerMaker.MakeMarkersByPOI(_map.POIs, markerSize))
            {
                NativeMap.AddMarker(marker);
            }
        }
    }
}