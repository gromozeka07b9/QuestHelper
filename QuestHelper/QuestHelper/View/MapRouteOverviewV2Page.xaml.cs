using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
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
            _vm.StartDialog();
            Task.Run(async () => { await mapControl.CenterMapOnLastPosition(); });
            Task.Run(async () => { await mapControl.UpdateTrackOnMap(_vm.GetTrackPlaces()); });
            Task.Run(async () => { await mapControl.UpdatePointsOnMap(_vm.GetRoutePoints()); });
        }

        private void MapRouteOverviewV2Page_OnDisappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
        }
    }
}