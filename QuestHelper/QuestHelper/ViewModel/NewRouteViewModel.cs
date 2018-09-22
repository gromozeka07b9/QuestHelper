using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.Model.DB;
using QuestHelper.View;
using QuestHelper.WS;
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
    public class NewRouteViewModel : INotifyPropertyChanged
    {
        private bool _splashStartScreenIsVisible;
        private Route _route;

        private bool _isFirstRoute;

        private RouteManager _routeManager = new RouteManager();
        private RoutePointManager _routePointManager = new RoutePointManager();
        private RoutePointMediaObjectManager _routePointMediaObjectManager = new RoutePointMediaObjectManager();

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ShowNewRouteDialogCommand { get; private set; }
        public ICommand OpenRoutePointDialogCommand { get; private set; }

        public NewRouteViewModel(Route route, bool isFirstRoute)
        {
            _route = route;
            _isFirstRoute = isFirstRoute;
            ShowNewRouteDialogCommand = new Command(showNewRouteData);
            OpenRoutePointDialogCommand = new Command(openRoutePointDialog);
        }

        private void openRoutePointDialog()
        {
            if (!_route.IsManaged)
            {
                _routeManager.Add(_route);
                Navigation.PushAsync(new RoutePage(_route, false));
            }
        }

        public void startDialog()
        {
            SplashStartScreenIsVisible = _isFirstRoute;
        }

        void showNewRouteData()
        {
            SplashStartScreenIsVisible = !SplashStartScreenIsVisible;
        }

        public string Name
        {
            set
            {
                if (_route.Name != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _route.Name = value;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _route.Name;
            }
        }

        public bool SplashStartScreenIsVisible
        {
            set
            {
                if (_splashStartScreenIsVisible != value)
                {
                    _splashStartScreenIsVisible = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SplashStartScreenIsVisible"));
                    }
                }
            }
            get
            {
                return _splashStartScreenIsVisible;
            }
        }
    }
}
