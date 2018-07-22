using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using QuestHelper.ViewModel;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        void showRoutes()
        {
            CustomMap mapOverview = (CustomMap)this.Content;
            var locator = CrossGeolocator.Current;
            var routePoints = vm.GetPointsForOverview();
            foreach(var point in routePoints)
            {
                mapOverview.RouteCoordinates.Add(new Position(point.Latitude, point.Longitude));
                var position1 = new Xamarin.Forms.Maps.Position(point.Latitude, point.Longitude);
                var pointPin = new Pin
                {                    
                    Type = PinType.Place,
                    Position = position1,
                    Label = point.Name,
                    Address = point.Address
                };
                pointPin.Clicked += PointPin_Clicked;
                mapOverview.Pins.Add(pointPin);
            }
        }

        private void PointPin_Clicked(object sender, EventArgs e)
        {
            var point = (Pin)sender;
            vm.OpenPointPropertiesAsync(point.Position.Latitude, point.Position.Longitude);
        }

        private async void ContentPage_AppearingAsync(object sender, EventArgs e)
        {
            var centerResult = await centerMapToCurrentPositionAsync(10);
            if (!centerResult)
            {
                var answerRetry = await DisplayAlert("Ошибка", "Не удалось определить ваше местоположение. Повторить поиск?", "Да", "Нет");
                if(answerRetry)
                {
                    var retryCenterResult = await centerMapToCurrentPositionAsync(60);
                    if(!retryCenterResult)
                    {
                        await DisplayAlert("Ошибка", "Не удалось определить ваше местоположение.", "Ок");
                    }
                }
            }
            showRoutes();
        }

        private async Task<bool> centerMapToCurrentPositionAsync(int timeout)
        {
            bool result = false;
            var locator = CrossGeolocator.Current;
            try
            {
                var currentPosition = await locator.GetPositionAsync(TimeSpan.FromSeconds(timeout));
                CustomMap mapOverview = (CustomMap)this.Content;
                mapOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(currentPosition.Latitude, currentPosition.Longitude), Distance.FromKilometers(1)));
                result = true;
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string> {{"Screen", "MapOverview"}, {"Action", "CenterToMap"} };
                Crashes.TrackError(exception, properties);
            }
            return result;
        }
    }
}
