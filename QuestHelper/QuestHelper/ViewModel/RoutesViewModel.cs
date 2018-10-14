using QuestHelper.Managers;
using QuestHelper.LocalDB.Model;
using QuestHelper.View;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.ViewModel
{
    class RoutesViewModel : INotifyPropertyChanged
    {
        private string _routeId;
        private IEnumerable<Route> _routes;
        private Route _routeItem;
        private RouteManager _routeManager = new RouteManager();
        private RoutesApiRequest _api = new RoutesApiRequest("http://questhelperserver.azurewebsites.net");
        private bool _noRoutesWarningIsVisible = false;
        private bool _isRefreshing = false;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand AddNewRouteCommand { get; private set; }
        public ICommand RefreshListRoutesCommand { get; private set; }

        public RoutesViewModel()
        {
            AddNewRouteCommand = new Command(addNewRouteCommandAsync);
            RefreshListRoutesCommand = new Command(refreshListRoutesCommand);
        }

        async void refreshListRoutesCommand()
        {
            IsRefreshing = true;
            Routes = _routeManager.GetRoutes();
            NoRoutesWarningIsVisible = Routes.Count() == 0;
            IsRefreshing = false;
#if DEBUG
            //List<Route> routes = await _api.GetRoutes();
            //_routeManager.UpdateLocalData(routes);
#else
            //List<Route> routes = await _api.GetRoutes();
            //_routeManager.UpdateLocalData(routes);
#endif
        }

        async void addNewRouteCommandAsync()
        {
            await Navigation.PushAsync(new NewRoutePage(new Route(), !Routes.Any()));
        }

        public bool IsRefreshing
        {
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsRefreshing"));
                    }
                }
            }
            get
            {
                return _isRefreshing;
            }
        }
        public bool NoRoutesWarningIsVisible
        {
            set
            {
                if (_noRoutesWarningIsVisible != value)
                {
                    _noRoutesWarningIsVisible = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("NoRoutesWarningIsVisible"));
                    }
                }
            }
            get
            {
                return _noRoutesWarningIsVisible;
            }
        }
        public string RouteId
        {
            set
            {
                if (_routeId != value)
                {
                    _routeId = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("RouteId"));
                    }
                }
            }
            get
            {
                return _routeId;
            }
        }
        public Route SelectedRouteItem
        {
            set
            {
                if (_routeItem != value)
                {
                    _routeItem = value;
                    Navigation.PushAsync(new RoutePage(value, false));
                    _routeItem = null;
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedRouteItem"));
                }
            }
        }
        public IEnumerable<Route> Routes
        {
            set
            {
                if (_routes != value)
                {
                    _routes = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Routes"));
                    }
                }
            }
            get
            {
                return _routes;
            }
        }
    }
}
