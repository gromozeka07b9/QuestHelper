using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace QuestHelper
{
    public partial class MapOverviewPage : ContentPage
    {
        public MapOverviewPage()
        {
            InitializeComponent();
            //centerMap();
        }

        async void centerMap()
        {
            var locator = CrossGeolocator.Current;
            var currentPosition = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            MapOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(currentPosition.Latitude,currentPosition.Longitude), Distance.FromKilometers(1)));
            /*var position1 = new Position(position.Latitude, position.Longitude); // Latitude, Longitude
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position1,
                Label = "привет",
                Address = "детали"
            };
            MyMap.Pins.Add(pin);*/
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            //centerMap();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            centerMap();
        }
    }
}
