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
    public class RouteCreatedViewModel : INotifyPropertyChanged
    {
        private Route _route;
        private RouteManager _routeManager = new RouteManager();

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand OpenRoutePointDialogCommand { get; private set; }

        public RouteCreatedViewModel(Route route)
        {
            _route = route;
            OpenRoutePointDialogCommand = new Command(openRoutePointDialog);
        }

        private void openRoutePointDialog()
        {
            Navigation.PushAsync(new RoutePage(_route, false));
        }

        public void startDialog()
        {
        }

        public string Name
        {
            get
            {
                return "Маршрут '" + _route.Name + "' создан.";
            }
        }
    }
}
