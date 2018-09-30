using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using QuestHelper.Model.DB;
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

        private bool centerMapToPositionAsync(double Latitude, double Longitude, int timeout)
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
                //pointPin.Clicked += PointPin_Clicked;
                mapOverview.Pins.Add(pointPin);
                result = true;
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string> { { "Screen", "RoutePointPage" }, { "Action", "CenterToMap" } };
                Crashes.TrackError(exception, properties);
            }
            return result;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if((_routePoint!=null)&&(_routePoint.Latitude > 0) && (_routePoint.Longitude > 0))
            {
                centerMapToPositionAsync(_routePoint.Latitude, _routePoint.Longitude, 15);
            }
        }
    }
}