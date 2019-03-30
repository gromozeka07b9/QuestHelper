﻿using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using QuestHelper.Model;
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
    public partial class MapRouteOverviewPage : ContentPage
    {
        MapRouteOverviewViewModel vm;
        public string CurrentRouteId = string.Empty;
        public MapRouteOverviewPage()
        {
            InitializeComponent();
        }
        public MapRouteOverviewPage(string routeId)
        {
            InitializeComponent();
            vm = new MapRouteOverviewViewModel(routeId);
            vm.Navigation = this.Navigation;
            BindingContext = vm;
            Analytics.TrackEvent("Map route showed");
        }

        private void PointPin_Clicked(object sender, EventArgs e)
        {
            var point = (Pin)sender;
            vm.OpenPointPropertiesAsync(point.Position.Latitude, point.Position.Longitude);
        }

        private async void ContentPage_AppearingAsync(object sender, EventArgs e)
        {
            var customMap = this.FindByName<CustomMap>("MapRouteOverview");
            var points = await vm.GetPointsForOverviewRouteAsync();
            customMap.Points = points.Select(x => new PointForMap()
            {
                Latitude = x.Latitude, Longitude = x.Longitude, PathToPicture = x.ImagePreviewPathForList, Name = x.Name, Description = x.Description
            }).ToList();
            CustomMapView customMapView = new CustomMapView(customMap, 15);
            if (customMap.Points.Count() > 0)
            {
                await centerMap(customMapView, customMap.Points.First().Latitude, customMap.Points.First().Longitude);
            }
        }

        private async Task getPositionAndCenterMap(CustomMapView customMap)
        {
            if (await customMap.GetPositionAsync())
            {
                await centerMap(customMap, customMap.CurrentPosition.Latitude, customMap.CurrentPosition.Longitude);
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool answerRetry = await DisplayAlert("Ошибка", customMap.LastError + " Повторить поиск?", "Да", "Нет");
                    if (answerRetry)
                    {
                        await centerMap(customMap, customMap.CurrentPosition.Latitude, customMap.CurrentPosition.Longitude);
                    }
                });
            }
        }

        private async Task centerMap(CustomMapView customMap, double Latitude, double Longitude)
        {
            if (!customMap.CenterMapToPosition(Latitude, Longitude, 10))
            {
                bool answerRetry = await DisplayAlert("Ошибка", customMap.LastError + " Повторить?", "Да", "Нет");
                if (answerRetry)
                {
                    await centerMap(customMap, Latitude, Longitude);
                }
            }
        }
    }
}
