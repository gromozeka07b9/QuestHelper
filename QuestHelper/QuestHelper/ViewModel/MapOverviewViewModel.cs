using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.Model.DB;
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

namespace QuestHelper.ViewModel
{
    public class MapOverviewViewModel : INotifyPropertyChanged
    {
        RoutePointManager _routePointManager;
        RouteManager _routeManager;
        IEnumerable<RoutePoint> _pointsForOverview;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        //public ICommand StartDialogCommand { get; private set; }
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
            if(point.MainRoute != null)
            {
                var routePointPage = new RoutePointPage(point.MainRoute, point);
                await Navigation.PushAsync(routePointPage, true);
            }
        }

        internal IEnumerable<RoutePoint> GetPointsForOverview()
        {
            var resultPoints = new List<RoutePoint>();
            var routes = _routeManager.GetRoutes();
            foreach(var route in routes)
            {
                var points = _routePointManager.GetPointsByRoute(route);
                foreach(var point in points)
                {
                    resultPoints.Add(point);
                }
            }
            return resultPoints;
        }

        /*public void startDialog()
{
   if (!string.IsNullOrEmpty(_route.Name))
   {
       showRouteData();
   }
   else
   {
       _route.Name = "Неизвестный маршрут";
       showNewRouteWarningDialog();
   }
   ListIsRefreshing = false;
}*/


        /*public bool ListIsRefreshing
        {
            set
            {
                if (_listIsRefreshing != value)
                {
                    _listIsRefreshing = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("ListIsRefreshing"));
                    }
                }
            }
            get
            {
                return _listIsRefreshing;
            }
        }
        public bool NoPointWarningIsVisible
        {
            set
            {
                if (_noPointWarningIsVisible != value)
                {
                    _noPointWarningIsVisible = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("NoPointWarningIsVisible"));
                    }
                }
            }
            get
            {
                return _noPointWarningIsVisible;
            }
        }*/
    }
}
