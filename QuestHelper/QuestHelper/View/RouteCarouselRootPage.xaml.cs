using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading.Forms;
using PanCardView;
using PanCardView.EventArgs;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.View.Converters;
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
        }

        private void RouteCarouselRootPage_OnAppearing(object sender, EventArgs e)
        {
            GC.Collect();
            MapRouteOverview.Points = _vm.PointsOnMap;
            _vm.StartDialogAsync();
        }

        private void RouteCarouselRootPage_OnDisappearing(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void Cards_ItemAppeared(CardsView view, ItemAppearedEventArgs args)
        {
            //ToDo: Так и не нашел понял, как картой управлять.
            if ((_vm.CurrentItem?.Latitude != 0) || (_vm.CurrentItem?.Longitude != 0))
            {
                MapRouteOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(_vm.CurrentItem.Latitude, _vm.CurrentItem.Longitude), Distance.FromKilometers(1)));
            }
        }
    }
}