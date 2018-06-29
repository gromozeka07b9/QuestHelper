using QuestHelper.Managers;
using QuestHelper.Model.DB;
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
        private List<string> _routes;
        private IEnumerable<Route> _routesObj;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand MapOverviewCommand { get; private set; }
        public ICommand RoutesCommand { get; private set; }
        public ICommand NewRouteCommand { get; private set; }
        public ICommand AroundMeCommand { get; private set; }
        public ICommand MyProfileCommand { get; private set; }
        public ICommand EditRouteCommand { get; private set; }

        public RoutesViewModel()
        {
            MapOverviewCommand = new Command(mapOverviewShow);
            RoutesCommand = new Command(routesShow);
            NewRouteCommand = new Command(newRouteShow);
            AroundMeCommand = new Command(aroundMeShow);
            MyProfileCommand = new Command(myProfileShow);
            //EditRouteCommand = new Command(editRoute);
            EditRouteCommand = new Command(editRoute);

            RouteManager manager = new RouteManager();

            _routesObj = manager.GetRoutes();
            _routes = new List<string>();
            foreach (var item in _routesObj)
            {
                _routes.Add(item.RouteId);
            }
        }

        async void editRoute()
        {
        }

        async void mapOverviewShow()
        {
            //this.Navigation = Application.Current.MainPage.Navigation;
            //await Navigation.PushAsync(new MapOverviewPage());
        }

        async void routesShow()
        {
            //this.Navigation = Application.Current.MainPage.Navigation;
            //await Navigation.PushAsync(new RoutesPage());
        }
        async void newRouteShow()
        {

        }
        async void aroundMeShow()
        {

        }
        async void myProfileShow()
        {

        }
        public List<string> Routes
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
        public IEnumerable<Route> RoutesObj
        {
            set
            {
                if (_routesObj != value)
                {
                    _routesObj = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("RoutesObj"));
                    }
                }
            }
            get
            {
                return _routesObj;
            }
        }
    }
}
