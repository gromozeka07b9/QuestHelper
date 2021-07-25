using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.View.Geo;
using QuestHelper.ViewModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapRouteOverviewV2Page : ContentPage
    {
        private readonly MapRouteOverviewV2ViewModel _vm;
        
        public MapRouteOverviewV2Page()
        {
            InitializeComponent();
        }
        
        //https://nerd-corner.com/how-to-add-custom-icon-pins-to-google-maps-xamarin-app/
        public MapRouteOverviewV2Page(string routeId)
        {
            InitializeComponent();
            _vm = new MapRouteOverviewV2ViewModel(routeId);
            _vm.Navigation = this.Navigation;
            BindingContext = _vm;
            mapControl.MoveToLastRegionOnLayoutChange = true;
        }

        private void MapRouteOverviewV2Page_OnAppearing(object sender, EventArgs e)
        {
            MessagingCenter.Subscribe<MapUpdateLocationPointMessage>(this, string.Empty, async (msgSender) =>
            {
                mapControl.UpdateLocationPointOnMap(msgSender.RoutePointId, msgSender.Latitude, msgSender.Longitude);
            });
            _vm.StartDialog();
            if (!string.IsNullOrEmpty(_vm.SelectedRoutePoint.Id))
            {
                Task.Run(async () =>
                {
                    await mapControl.CenterMap(_vm.SelectedRoutePoint.Latitude, _vm.SelectedRoutePoint.Longitude);
                });
            }
            else
            {
                var points = _vm.GetRoutePoints();
                var firstPoint = points.FirstOrDefault();
                if(firstPoint != null)
                {
                    Task.Run(async () => { await mapControl.CenterMap(firstPoint.Latitude, firstPoint.Longitude); });
                }
                else
                {
                    Task.Run(async () => { await mapControl.CenterMapOnLastPosition(); });
                }
                Task.Run(async () =>
                {
                    mapControl.RoutePoints = points;
                    var trackPlaces = _vm.GetTrackPlaces();
                    if (trackPlaces.Any())
                    {
                        await mapControl.UpdateTrackOnMap(trackPlaces);
                    }
                    mapControl.IsShowConnectedRoutePointsLines = !trackPlaces.Any();
                    await mapControl.UpdatePointsOnMap(RoutePoint_MarkerClicked);
                });
            }
        }

        private void RoutePoint_MarkerClicked(object sender, PinClickedEventArgs e)
        {
            var selectedPin = (RoutePointPin)sender;            
            _vm.SelectRoutePointPin(selectedPin.RoutePointId);
        }

        private void MapRouteOverviewV2Page_OnDisappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
            MessagingCenter.Unsubscribe<MapUpdateLocationPointMessage>(this, string.Empty);
        }

        private void MapControl_OnMapClicked(object sender, MapClickedEventArgs e)
        {
            _vm.SetNewLocation(e.Position);
        }
    }
}