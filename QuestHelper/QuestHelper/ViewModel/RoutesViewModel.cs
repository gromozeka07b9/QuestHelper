using QuestHelper.Managers;
using QuestHelper.Model.DB;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public RoutesViewModel()
        {
            RouteManager manager = new RouteManager();

            _routes = manager.GetRoutes();
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
