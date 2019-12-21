using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PanCardView;
using PanCardView.EventArgs;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
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
        private const string _apiUrl = "http://igosh.pro/api";
        private string _authToken;
        private RouteCarouselRootViewModel _vm;
        public RouteCarouselRootPage(string routeId, string routePointId = "")
        {
            InitializeComponent();
            _vm = new RouteCarouselRootViewModel(routeId, routePointId) { Navigation = this.Navigation };
            BindingContext = _vm;
        }

        private async void RouteCarouselRootPage_OnAppearingAsync(object sender, EventArgs e)
        {
            TokenStoreService tokenService = new TokenStoreService();
            _authToken = await tokenService.GetAuthTokenAsync();
            MapRouteOverview.Points = _vm.PointsOnMap;
            _vm.StartDialogAsync();
        }

        private void Cards_OnItemAppearing(CardsView view, ItemAppearingEventArgs args)
        {
            var newItem = (RouteCarouselRootViewModel.CarouselItem) view.SelectedItem;
            if (!newItem.IsFullImage)
            {
                Device.StartTimer(TimeSpan.FromMilliseconds(500), OnTimerForUpdate);
            }

            if ((_vm.CurrentItem?.Latitude != newItem.Latitude)||(_vm.CurrentItem?.Longitude != newItem.Longitude))
            {
                MapRouteOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(newItem.Latitude, newItem.Longitude), Distance.FromKilometers(3)));
            }

            _vm.CurrentItem = newItem;

        }

        private void RouteCarouselRootPage_OnDisappearing(object sender, EventArgs e)
        {
        }
        private bool OnTimerForUpdate()
        {
            string fullImgPath = ImagePathManager.GetImagePath(_vm.CurrentItem.MediaId, MediaObjectTypeEnum.Image, false);
            if (File.Exists(fullImgPath))
            {
                _vm.CurrentItem.ImageSource = fullImgPath;
                _vm.CurrentItem.IsFullImage = true;
            }
            else
            {
                if (_vm.IsMaximumQualityPhoto)
                {
                    _vm.LoadFullImage(fullImgPath);
                }
            }
            return false;
        }
    }
}