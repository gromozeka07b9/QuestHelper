using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using QuestHelper.View.Geo;
using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutePage : ContentPage
	{
        RouteViewModel vm;
        public RoutePage()
		{
            InitializeComponent ();
            vm = new RouteViewModel(string.Empty, false) { Navigation = this.Navigation };
            BindingContext = vm;
        }
        public RoutePage(Route routeItem, bool isFirstRoute)
        {

            InitializeComponent();
            Title = routeItem.Name;
            vm = new RouteViewModel(routeItem.RouteId, isFirstRoute) { Navigation = this.Navigation };
            BindingContext = vm;
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            vm.startDialog();
            if (vm.PointsOfRoute.Count > 0)
            {
                var centerPoint = vm.PointsOfRoute[0];
                await ShowMapAsync(vm.PointsOfRoute);
            }
        }

        private async Task ShowMapAsync(ObservableCollection<ViewRoutePoint> pointsOfRoute)
        {
            if (pointsOfRoute.Count > 0)
            {
                var centerPoint = vm.PointsOfRoute[0];
                CustomMap custMap = (CustomMap) this.PointMapOverview;
                custMap.RouteCoordinates = vm.PointsOfRoute.Select(x => new Position(x.Latitude, x.Longitude)).ToList();
                CustomMapView customMap = new CustomMapView(custMap, 15);
                
                if (customMap.CenterMapToPosition(centerPoint.Latitude, centerPoint.Longitude, 5))
                {
                    for (int i = 0; i < vm.PointsOfRoute.Count; i++)
                    {
                        var point = vm.PointsOfRoute[i];
                        if (i == 0)
                        {
                            customMap.AddPin(point.Latitude, point.Longitude, "Старт", point.Address, PointPin_Clicked);
                        } else if (i == vm.PointsOfRoute.Count - 1)
                        {
                            customMap.AddPin(point.Latitude, point.Longitude, "Финиш", point.Address, PointPin_Clicked);
                        }
                        else
                        {
                            customMap.AddPin(point.Latitude, point.Longitude, point.Name, point.Address, PointPin_Clicked);
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", customMap.LastError, "Ок");
                }
            }
        }

        /*private async Task CenterMap(double latitude, double longitude, string name, string address)
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
	    }*/

        private void PointPin_Clicked(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}