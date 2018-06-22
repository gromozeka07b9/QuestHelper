using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QuestHelper.ViewModel
{
    class RoutePointViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand CreateCommand { get; private set; }
        public ICommand UpdateCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public RoutePointViewModel()
        {
            CreateCommand = new Command(createRoutePoint);
            UpdateCommand = new Command(updateRoutePoint);
            DeleteCommand = new Command(deleteRoutePoint);
        }

        async void createRoutePoint()
        {
        }

        async void updateRoutePoint()
        {
        }
        async void deleteRoutePoint()
        {

        }
    }
}
