using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.LocalDB.Model;
using QuestHelper.View;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuestHelper.Model;

namespace QuestHelper.ViewModel
{
    public class MapOverviewViewModel : INotifyPropertyChanged
    {
        RoutePointManager _routePointManager;
        RouteManager _routeManager;
        //IEnumerable<RoutePoint> _pointsForOverview;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand OpenPointPropertiesCommand { get; private set; }

        public MapOverviewViewModel()
        {
            OpenPointPropertiesCommand = new Command(openPointPropertiesCommand);
            _routePointManager = new RoutePointManager();
            _routeManager = new RouteManager();
        }

        internal void openPointPropertiesCommand()
        {

        }
        internal async void OpenPointPropertiesAsync(double latitude, double longitude)
        {
            RoutePoint point = _routePointManager.GetPointByCoordinates(latitude, longitude);
            if(point != null && point.MainRoute != null)
            {
                var routePointPage = new RoutePointPage(point.MainRoute.RouteId, point.RoutePointId);
                await Navigation.PushAsync(routePointPage, true);
            }
        }
    }
}
