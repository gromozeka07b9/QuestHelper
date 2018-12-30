using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using QuestHelper.View.Geo;
using QuestHelper.ViewModel;
using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace QuestHelper.View
{
    public partial class MapOverviewPage : ContentPage
    {
        MapOverviewViewModel vm;
        public MapOverviewPage()
        {
            InitializeComponent();
            vm = new MapOverviewViewModel();
            vm.Navigation = this.Navigation;
            BindingContext = vm;
            Analytics.TrackEvent("Map showed");
        }

        private void PointPin_Clicked(object sender, EventArgs e)
        {
            var point = (Pin)sender;
            vm.OpenPointPropertiesAsync(point.Position.Latitude, point.Position.Longitude);
        }

        private async void ContentPage_AppearingAsync(object sender, EventArgs e)
        {
            CustomMapView customMap = new CustomMapView((CustomMap)this.Content, 15);
            var routePoints = vm.GetPointsForOverview();
            foreach (var point in routePoints)
            {
                customMap.AddPin(point.Latitude, point.Longitude, point.Name, point.Address, PointPin_Clicked);
            }

            await centerMap(customMap);
        }

        private async Task centerMap(CustomMapView customMap)
        {
            if (await customMap.GetPositionAsync())
            {
                if (!customMap.CenterMapToPosition(customMap.CurrentPosition.Latitude, customMap.CurrentPosition.Longitude))
                {
                    bool answerRetry = await DisplayAlert("Ошибка", customMap.LastError + " Повторить?", "Да", "Нет");
                    if (answerRetry)
                    {
                        await centerMap(customMap);
                    }
                }
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool answerRetry = await DisplayAlert("Ошибка", customMap.LastError + " Повторить поиск?", "Да", "Нет");
                    if (answerRetry)
                    {
                        await centerMap(customMap);
                    }
                });
            }
        }
    }
}
