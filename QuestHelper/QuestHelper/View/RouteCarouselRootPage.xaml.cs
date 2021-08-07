using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using PanCardView;
using PanCardView.EventArgs;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.View.Converters;
using QuestHelper.View.Geo;
using QuestHelper.ViewModel;
using QuestHelper.WS;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace QuestHelper.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RouteCarouselRootPage : ContentPage
    {
        private RouteCarouselRootViewModel _vm;
        public RouteCarouselRootPage(string routeId)
        {
            InitializeComponent();
            _vm = new RouteCarouselRootViewModel(routeId) { Navigation = this.Navigation };
            BindingContext = _vm;
            _vm.PropertyChanged += _vm_PropertyChanged;
        }

        private void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "IsMapShow")
            {
                centerMap();
            }
        }

        private async void RouteCarouselRootPage_OnAppearing(object sender, EventArgs e)
        {
            GC.Collect();
            //MapRouteOverview.Points = _vm.PointsOnMap;
            MapRouteOverviewTrackMap.RoutePoints = _vm.RoutePoints;
            var trackPlaces = _vm.GetTrackPlaces();
            if (trackPlaces.Any())
            {
                await MapRouteOverviewTrackMap.UpdateTrackOnMap(trackPlaces);
            }
            MapRouteOverviewTrackMap.IsShowConnectedRoutePointsLines = !trackPlaces.Any();
            await MapRouteOverviewTrackMap.UpdatePointsOnMap(RoutePoint_MarkerClicked);

            _vm.StartDialogAsync();
        }

        private void RouteCarouselRootPage_OnDisappearing(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void RoutePoint_MarkerClicked(object sender, PinClickedEventArgs e)
        {
            var selectedPin = (RoutePointPin)sender;            
            _vm.SelectRoutePointPin(selectedPin.RoutePointId);
        }

        private void Cards_ItemAppeared(CardsView view, ItemAppearedEventArgs args)
        {
            centerMap();
        }

        private void centerMap()
        {
            //ToDo: Так и не нашел понял, как картой управлять через вьюмодель.
            if ((_vm.CurrentItem?.Latitude != 0) || (_vm.CurrentItem?.Longitude != 0))
            {
                //MapRouteOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(_vm.CurrentItem.Latitude, _vm.CurrentItem.Longitude), Distance.FromKilometers(1)));
                if ((_vm.CurrentItem != null) && (_vm.CurrentItem.Latitude != 0) && (_vm.CurrentItem.Longitude != 0))
                {
                    MapRouteOverviewTrackMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(_vm.CurrentItem.Latitude, _vm.CurrentItem.Longitude), Distance.FromKilometers(1)));
                }
            }
        }
    }
}