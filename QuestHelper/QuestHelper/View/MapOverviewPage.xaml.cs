using Plugin.Geolocator;
using System;
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
            var locator = CrossGeolocator.Current;
            var currentPosition = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            MapOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(currentPosition.Latitude,currentPosition.Longitude), Distance.FromKilometers(1)));
            /*var position1 = new Xamarin.Forms.Maps.Position(currentPosition.Latitude, currentPosition.Longitude); // Latitude, Longitude
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position1,
                Label = "привет",
                Address = "детали"
            };
            MapOverview.Pins.Add(pin);*/
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            centerMap();
        }
    }
}
