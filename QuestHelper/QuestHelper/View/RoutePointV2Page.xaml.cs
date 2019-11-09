using QuestHelper.Managers;
using QuestHelper.Model.Messages;
using QuestHelper.View.Geo;
using QuestHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoutePointV2Page : ContentPage
    {
        IToolbarService _toolbarService = DependencyService.Get<IToolbarService>();
        private RoutePointV2ViewModel _vm;
        
        public RoutePointV2Page()
        {
            InitializeComponent();
        }
        public RoutePointV2Page(string routeId, string routePointId)
        {
            InitializeComponent();
            RoutePointManager manager = new RoutePointManager();
            _vm = new RoutePointV2ViewModel(routeId, routePointId) 
            { 
                Navigation = this.Navigation 
            };
            BindingContext = _vm;
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            _toolbarService.SetVisibilityToolbar(false);
            _vm.StartDialog();
            await CenterMap(_vm.Latitude, _vm.Longitude, _vm.Name, _vm.Address);
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
            //MessagingCenter.Unsubscribe<MapUpdateLocationPointMessage>(this, string.Empty);
        }

        private async Task CenterMap(double latitude, double longitude, string name, string address)
        {
            CustomMapView customMap = new CustomMapView((CustomMap)this.PointMapOverview, 15);
            if (customMap.CenterMapToPosition(latitude, longitude, 1))
            {
                customMap.ClearPins();
                customMap.AddPin(latitude, longitude, name, address, PointPin_Clicked);
            }
            else
            {
                await DisplayAlert("Error while centering map", customMap.LastError, "Ок");
            }
        }

        private void PointPin_Clicked(object sender, EventArgs e)
        {
        }

    }
}