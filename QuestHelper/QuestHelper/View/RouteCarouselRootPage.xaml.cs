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
        public RouteCarouselRootPage(string routeId)
        {
            InitializeComponent();
            _vm = new RouteCarouselRootViewModel(routeId) { Navigation = this.Navigation };
            BindingContext = _vm;
        }

        private async void RouteCarouselRootPage_OnAppearingAsync(object sender, EventArgs e)
        {
            GC.Collect();
            //TokenStoreService tokenService = new TokenStoreService();
            //_authToken = await tokenService.GetAuthTokenAsync();
            MapRouteOverview.Points = _vm.PointsOnMap;
            _vm.StartDialogAsync();
        }

        private void RouteCarouselRootPage_OnDisappearing(object sender, EventArgs e)
        {
            GC.Collect();
        }

        private void Cards_ItemAppeared(CardsView view, ItemAppearedEventArgs args)
        {
            //ToDo: Так и не нашел способа сделать горизонтальный список. Ни ListView, ни CarouselView не позволяют.
            double previewWidthRequest = 80;
            double previewHeightRequest = 80;
            StackPreviewImages.Children.Clear();
            if (_vm.CurrentPointImagesPreview != null)
            {
                var listImages = _vm.CurrentPointImagesPreview.Select(img => new CustomCachedImage() 
                { 
                    Source = (MediaObjectTypeEnum)img.MediaType == MediaObjectTypeEnum.Audio ? "sound.png" : img.ImageSource,
                    Aspect = Aspect.AspectFill, 
                    DownsampleToViewSize = true, 
                    WidthRequest = previewWidthRequest, 
                    HeightRequest = previewHeightRequest, 
                    RoutePointId = img.RoutePointId, 
                    RoutePointMediaId = img.RoutePointMediaId,
                    MediaType = img.MediaType
                });
                if (listImages.Count() > 1)
                {
                    foreach (var imgItem in listImages)
                    {
                        imgItem.GestureRecognizers.Add(new TapGestureRecognizer() { Command = _vm.ShowOtherPhotoCommand, CommandParameter = imgItem });
                        StackPreviewImages.Children.Add(imgItem);
                    }
                }
            }

            if ((_vm.CurrentItem?.Latitude != 0) || (_vm.CurrentItem?.Longitude != 0))
            {
                MapRouteOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(_vm.CurrentItem.Latitude, _vm.CurrentItem.Longitude), Distance.FromKilometers(1)));
            }
        }
    }
}