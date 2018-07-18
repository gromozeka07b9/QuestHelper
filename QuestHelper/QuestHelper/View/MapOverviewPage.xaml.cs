using Plugin.Geolocator;
using QuestHelper.ViewModel;
using Realms;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace QuestHelper.View
{
    public partial class MapOverviewPage : ContentPage
    {
        MapOverviewViewModel vm;
        public MapOverviewPage()
        {
            InitializeComponent();
            vm = new MapOverviewViewModel();
            vm.Navigation = this.Navigation;
            BindingContext = vm;

        }

        async void centerMap()
        {
            var customMap = new CustomMap
            {
                MapType = MapType.Street
            };
            var locator = CrossGeolocator.Current;
            var routePoints = vm.GetPointsForOverview();
            foreach(var point in routePoints)
            {
                
                customMap.RouteCoordinates.Add(new Position(point.Latitude, point.Longitude));
                var position1 = new Xamarin.Forms.Maps.Position(point.Latitude, point.Longitude);
                var pointPin = new Pin
                {
                    Type = PinType.Place,
                    Position = position1,
                    Label = point.Name,
                    Address = point.Address,                    
                };
                customMap.Pins.Add(pointPin);
            }
            Content = customMap;
            var currentPosition = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(currentPosition.Latitude, currentPosition.Longitude), Distance.FromKilometers(1)));
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            centerMap();
        }
    }
}
