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
            RoutePointManager manager = new RoutePointManager();
            _vm = new RoutePointV2ViewModel(routeId, routePointId) 
            { 
                Navigation = this.Navigation 
            };
            _vm.PropertyChanged += Vm_PropertyChanged;
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

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            Task.Run(async () => 
            {
                PermissionManager permissions = new PermissionManager();

                if (!await permissions.PermissionGetCoordsGrantedAsync())
                {
                    await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Location, CommonResource.Permission_Position);
                }
            });

            _vm.StartDialog();
            MessagingCenter.Subscribe<MapUpdateLocationPointMessage>(this, string.Empty, async (msgSender) =>
            {
                await CenterMap(_vm.Latitude, _vm.Longitude, _vm.Name, _vm.Address);
            });
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
            MessagingCenter.Unsubscribe<MapUpdateLocationPointMessage>(this, string.Empty);
        }

        private async Task CenterMap(double latitude, double longitude, string name, string address)
        {
            CustomMapView customMap = new CustomMapView(PointMapOverview, 15);
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

        private void Image_Unfocused(object sender, FocusEventArgs e)
        {
            NameEditorCtrl.Unfocus();
        }
    }
}