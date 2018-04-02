using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.ViewModel
{
    class NewRouteViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand MapOverviewCommand { get; private set; }
        public ICommand RoutesCommand { get; private set; }
        public ICommand NewRouteCommand { get; private set; }
        public ICommand AroundMeCommand { get; private set; }
        public ICommand MyProfileCommand { get; private set; }

        public NewRouteViewModel()
        {
            MapOverviewCommand = new Command(mapOverviewShow);
            RoutesCommand = new Command(routesShow);
            NewRouteCommand = new Command(newRouteShow);
            AroundMeCommand = new Command(aroundMeShow);
            MyProfileCommand = new Command(myProfileShow);
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
    }
}
