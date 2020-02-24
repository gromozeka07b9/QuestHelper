using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using QuestHelper.Model;
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
            BindingContext = _vm;
        }

        public ObservableCollection<POI> GetPOIs()
        {
            return _vm.POIs;
        }
        /*private void PointPin_Clicked(object sender, EventArgs e)
        {
            var point = (Pin)sender;
            vm.OpenPointPropertiesAsync(point.Position.Latitude, point.Position.Longitude);
        }*/

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            _vm.StartDialog();
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            _vm.CloseDialog();
        }

        /*private async Task centerMap(CustomMapView customMap, double Latitude, double Longitude)
        {
            if (!customMap.CenterMapToPosition(Latitude, Longitude, 10))
            {
                bool answerRetry = await DisplayAlert("Ошибка", customMap.LastError + " Повторить?", "Да", "Нет");
                if (answerRetry)
                {
                    await centerMap(customMap, Latitude, Longitude);
                }
            }
        }*/
    }
}
