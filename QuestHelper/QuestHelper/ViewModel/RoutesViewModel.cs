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
            EditRouteCommand = new Command<string>(editRoute);

            RouteManager manager = new RouteManager();

            _routes = manager.GetRoutes();
            /*foreach (var item in _routesObj)
            {
                item.Name = "test";
            }*/
        }

        async void editRoute(string routeId)
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
                    Navigation.PushAsync(new EditRoutePage(value));

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
