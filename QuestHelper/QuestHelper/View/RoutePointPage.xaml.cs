using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.View.Geo;
using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RoutePointPage : ContentPage
	{
        private RoutePoint _routePoint;
        private RoutePointViewModel _vm;

        public ICommand Test { get; }

        public RoutePointPage()
        {
            InitializeComponent();
        }
        public RoutePointPage (string routeId, string routePointId)
		{
			InitializeComponent ();
            RoutePointManager manager = new RoutePointManager();
            if(!string.IsNullOrEmpty(routePointId))
                _routePoint = manager.GetPointById(routePointId);
            _vm = new RoutePointViewModel(routeId, routePointId) { Navigation = this.Navigation };
            _vm.PropertyChanged += Vm_PropertyChanged;
            _vm.DeleteCommand = new Command(deleteRoutePoint);
            BindingContext = _vm;
        }

        private async void deleteRoutePoint(object obj)
        {
            var result = await DisplayAlert("Внимание!", "Удалить точку?", "Да", "Нет");
            if(result)
            {
                _vm.DeletePoint();
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if((e.PropertyName == "Latitude")||(e.PropertyName == "Longitude"))
            {
                if((_vm.Latitude > 0)&& (_vm.Longitude > 0))
                {
                    CenterMap(_vm.Latitude, _vm.Longitude, _vm.Name, _vm.Address);
                }
            }
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
                await CenterMap(_routePoint.Latitude, _routePoint.Longitude, _routePoint.Name, _routePoint.Address);
            }
            _vm.StartDialog();
        }

        private async Task CenterMap(double latitude, double longitude, string name, string address)
        {
            CustomMapView customMap = new CustomMapView((CustomMap)this.PointMapOverview, 15);
            if (customMap.CenterMapToPosition(latitude, longitude))
            {
                customMap.AddPin(latitude, longitude, name, address, PointPin_Clicked);
            }
            else
            {
                await DisplayAlert("Ошибка", customMap.LastError, "Ок");
            }
        }
    }
}