using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanCardView;
using PanCardView.EventArgs;
using QuestHelper.Managers;
using QuestHelper.ViewModel;
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
            MapRouteOverview.Points = _vm.PointsOnMap;
            var toolbarService = DependencyService.Get<IToolbarService>();
            toolbarService.SetDarkMode(true);
            _vm.StartDialog();
        }

        private void Cards_OnItemAppearing(CardsView view, ItemAppearingEventArgs args)
        {
            var newItem = (RouteCarouselRootViewModel.CarouselItem) view.SelectedItem;
            if ((_vm.CurrentItem?.Latitude != newItem.Latitude)||(_vm.CurrentItem?.Longitude != newItem.Longitude))
            {
                MapRouteOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(newItem.Latitude, newItem.Longitude), Distance.FromKilometers(3)));
            }
            _vm.CurrentItem = newItem;
        }

        private void RouteCarouselRootPage_OnDisappearing(object sender, EventArgs e)
        {
            var toolbarService = DependencyService.Get<IToolbarService>();
            toolbarService.SetDarkMode(false);
        }
    }
}