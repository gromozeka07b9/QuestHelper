using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using QuestHelper.Model;
using QuestHelper.Resources;
using QuestHelper.View.Geo;
using QuestHelper.ViewModel;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace QuestHelper.View
{
    public partial class MapOverviewPage : ContentPage
    {
        MapOverviewViewModel _vm;
        public MapOverviewPage()
        {
            InitializeComponent();
            _vm = new MapOverviewViewModel();
            _vm.Navigation = this.Navigation;
            _vm.PropertyChanged += _vm_PropertyChanged;
            BindingContext = _vm;
        }

        private void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "POIs":
                    {
                        //MapOverview.Pins.Clear();
                        UpdatePinsFromPOIs();
                    }; break;
                case "CurrentLocation":
                    {
                        if ((_vm.CurrentLocation.Latitude != 0) && (_vm.CurrentLocation.Longitude != 0))
                        {
                            Task.Run(async () => {
                                await centerMap(_vm.CurrentLocation.Latitude, _vm.CurrentLocation.Longitude);
                            });
                        }
                    }; break;
            }
        }

        private void UpdatePinsFromPOIs()
        {
            MapOverview.Pins.Clear();
            foreach(var poi in _vm.POIs.Select(p => new OverViewMapPin() { Label = p.Name, Position = p.Location, ImagePath = "/resource/about.png", ClassId = "test", StyleId = "teststyle" }))
            {
                MapOverview.Pins.Add(poi);
            }
            //_vm.POIs.Select(p => new Pin() { Label = p.Name, Position = p.Location }).ToList();
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            _vm.StartDialog();
            //MapOverview.POIs = _vm.POIs.ToList();
            //MapOverview.Pins.Add(new Pin() { Label = "test"});
            //POI = new POI() { };
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
        }

        private async Task centerMap(double latitude, double longitude)
        {
            if (!centerMapToPosition(latitude, longitude, 100))
            {
                bool answerRetry = await DisplayAlert(CommonResource.CommonMsg_Warning, CommonResource.CommonMsg_Repeat + "?", CommonResource.CommonMsg_Yes, CommonResource.CommonMsg_No);
                if (answerRetry)
                {
                    centerMapToPosition(latitude, longitude, 100);
                }
            }
        }

        private bool centerMapToPosition(double Latitude, double Longitude, double ScaleKilometers)
        {
            bool result = false;
            try
            {
                MapOverview.MoveToRegion(MapSpan.FromCenterAndRadius(new Xamarin.Forms.Maps.Position(Latitude, Longitude), Distance.FromKilometers(ScaleKilometers)));
                result = true;
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string> { { "Action", "centerMapToPosition" } };
                Crashes.TrackError(exception, properties);
            }
            return result;
        }

    }
}
