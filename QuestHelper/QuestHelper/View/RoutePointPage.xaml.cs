using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using QuestHelper.LocalDB.Model;
using QuestHelper.View.Geo;
using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoutePointPage : ContentPage
	{
        private RoutePoint _routePoint;
        public RoutePointPage()
        {
            InitializeComponent();
        }
        public RoutePointPage (Route route, RoutePoint routePoint)
		{
			InitializeComponent ();
            _routePoint = routePoint;
            BindingContext = new RoutePointViewModel(route, routePoint) { Navigation = this.Navigation };
        }

        /*private bool centerMapToPositionAsync(double Latitude, double Longitude, int timeout)
        {
            bool result = false;
            var locator = CrossGeolocator.Current;
            try
            {
                CustomMap mapOverview = (CustomMap)this.PointMapOverview;
                mapOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(Latitude, Longitude), Distance.FromKilometers(1)));
                var position1 = new Xamarin.Forms.Maps.Position(Latitude, Longitude);
                var pointPin = new Pin
                {
                    Type = PinType.Place,
                    Position = position1,
                    Label = _routePoint.Name,
                    Address = _routePoint.Address
                };
                pointPin.Clicked += PointPin_Clicked; ;
                mapOverview.Pins.Add(pointPin);
                result = true;
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string> { { "Screen", "RoutePointPage" }, { "Action", "CenterToMap" } };
                Crashes.TrackError(exception, properties);
            }
            return result;
        }*/

        private void PointPin_Clicked(object sender, EventArgs e)
        {
        }

        private async void ContentPage_AppearingAsync(object sender, EventArgs e)
        {
            if((_routePoint!=null)&&(_routePoint.Latitude > 0) && (_routePoint.Longitude > 0) && (!string.IsNullOrEmpty(_routePoint.Name)))
            {
                CustomMapView customMap = new CustomMapView((CustomMap)this.PointMapOverview, 15);
                if(customMap.CenterMapToPosition(_routePoint.Latitude, _routePoint.Longitude))
                {
                    customMap.AddPin(_routePoint.Latitude, _routePoint.Longitude, _routePoint.Name, _routePoint.Address, PointPin_Clicked);
                } else
                {
                    await DisplayAlert("Ошибка", customMap.LastError, "Ок");
                }
            }
        }
    }
}