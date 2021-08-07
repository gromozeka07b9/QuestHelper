using QuestHelper.Managers;
using QuestHelper.Model.Messages;
using QuestHelper.Resources;
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
    public partial class RoutePointV2Page : ContentPage
    {
        private RoutePointV2ViewModel _vm;
        
        public RoutePointV2Page()
        {
            InitializeComponent();
        }
        public RoutePointV2Page(string routeId, string routePointId)
        {
            InitializeComponent();
            _vm = new RoutePointV2ViewModel(routeId, routePointId) 
            { 
                Navigation = this.Navigation 
            };
            BindingContext = _vm;
        }
        private async void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Coordinates":
                    {
                        if ((_vm.Latitude != 0) && (_vm.Longitude != 0))
                        {
                            await CenterMap(_vm.Latitude, _vm.Longitude, _vm.Name, _vm.Address);
                            _vm.ApplyChanges();
                        }
                    }; break;
            }
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            _vm.PropertyChanged += Vm_PropertyChanged;
            /*await Task.Run(async () => 
            {
                PermissionManager permissions = new PermissionManager();

                if (!await permissions.PermissionGetCoordsGrantedAsync())
                {
                    _vm.IsRightsToGetLocationPresented = await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Location, CommonResource.Permission_Position);
                }
            });*/
            PermissionManager permissions = new PermissionManager();

            if (!await permissions.PermissionGetCoordsGrantedAsync())
            {
                _vm.IsRightsToGetLocationPresented = await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Location, CommonResource.Permission_Position);
            }

            _vm.StartDialog();
            //PointMapOverview.RoutePoints = _vm.RoutePoints;
            //await PointMapOverview.UpdatePointsOnMap(RoutePoint_MarkerClicked);
            //await CenterMap(_vm.Latitude, _vm.Longitude, _vm.Name, _vm.Address);
            MessagingCenter.Subscribe<MapUpdateLocationPointMessage>(this, string.Empty, async (msgSender) =>
            {
                await CenterMap(_vm.Latitude, _vm.Longitude, _vm.Name, _vm.Address);
            });
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            _vm.PropertyChanged -= Vm_PropertyChanged;
            _vm.CloseDialog();
            MessagingCenter.Unsubscribe<MapUpdateLocationPointMessage>(this, string.Empty);
        }

        private async Task CenterMap(double latitude, double longitude, string name, string address)
        {
            CustomMapView customMap = new CustomMapView(PointMapOverview, 15);
            if (customMap.CenterMapToPosition(latitude, longitude, 1))
            {
                customMap.ClearPins();
                customMap.AddPin(latitude, longitude, name, address);
            }
            else
            {
                await DisplayAlert("Error while centering map", customMap.LastError, "Ок");
            }
            //await PointMapOverview.CenterMap(latitude, longitude);
            /*mapControl.RoutePoints = points;
            var trackPlaces = _vm.GetTrackPlaces();
            if (trackPlaces.Any())
            {
                await mapControl.UpdateTrackOnMap(trackPlaces);
            }
            mapControl.IsShowConnectedRoutePointsLines = !trackPlaces.Any();
            await mapControl.UpdatePointsOnMap(RoutePoint_MarkerClicked);*/
        }

        private void RoutePoint_MarkerClicked(object sender, PinClickedEventArgs e)
        {
            var selectedPin = (RoutePointPin)sender;            
            //_vm.SelectRoutePointPin(selectedPin.RoutePointId);
        }


        private void Image_Unfocused(object sender, FocusEventArgs e)
        {
            NameEditorCtrl.Unfocus();
        }
    }
}