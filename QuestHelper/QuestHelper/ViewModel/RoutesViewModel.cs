using QuestHelper.Managers;
using QuestHelper.Model.DB;
using QuestHelper.View;
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
        private bool _noRoutesWarningIsVisible;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand AddNewRouteCommand { get; private set; }

        public RoutesViewModel()
        {
            AddNewRouteCommand = new Command(addNewRouteCommandAsync);
            RouteManager manager = new RouteManager();

            _routes = manager.GetRoutes();
            NoRoutesWarningIsVisible = _routes.Count() == 0;
        }

        async void addNewRouteCommandAsync()
        {
            await Navigation.PushAsync(new RoutePage(new Route()));
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
                    Navigation.PushAsync(new RoutePage(value));
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
