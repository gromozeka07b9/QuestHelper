using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using QuestHelper.Droid;
using QuestHelper.Managers;
using QuestHelper.Resources;
using QuestHelper.View;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

[assembly: ExportRenderer(typeof(CustomOverviewMap), typeof(CustomOverviewMapRenderer))]
namespace QuestHelper.Droid
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
    }
}