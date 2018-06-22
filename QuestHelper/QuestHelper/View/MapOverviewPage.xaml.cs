using Plugin.Geolocator;
using Realms;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace QuestHelper.View
{
    public partial class MapOverviewPage : ContentPage
    {
        public MapOverviewPage()
        {
            InitializeComponent();
            centerMap();
        }

        async void centerMap()
        {
            var realm = Realm.GetInstance();
            IQueryable<Model.DB.RoutePoint> points = realm.All<Model.DB.RoutePoint>();
            var lastPosition = points.FirstOrDefault();

            var locator = CrossGeolocator.Current;
            MapOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(lastPosition.Latitude, lastPosition.Longitude), Distance.FromKilometers(1)));
            var position1 = new Xamarin.Forms.Maps.Position(lastPosition.Latitude, lastPosition.Longitude); // Latitude, Longitude
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position1,
                Label = "Точка",
                Address = "Ваше недавнее местоположение"
            };
            MapOverview.Pins.Add(pin);
            var currentPosition = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            MapOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(currentPosition.Latitude, currentPosition.Longitude), Distance.FromKilometers(1)));
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            //centerMap();
        }
    }
}
