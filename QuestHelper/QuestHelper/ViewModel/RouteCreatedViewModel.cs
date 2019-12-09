using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.LocalDB.Model;
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
using QuestHelper.Model;
using QuestHelper.Resources;

namespace QuestHelper.ViewModel
{
    public class RouteCreatedViewModel : INotifyPropertyChanged
    {
        private ViewRoute _vroute;
        private RouteManager _routeManager = new RouteManager();

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand OpenRoutePointDialogCommand { get; private set; }

        public RouteCreatedViewModel(string routeId)
        {
            _vroute = new ViewRoute(routeId);
            //_vroute.Load(routeId);
            OpenRoutePointDialogCommand = new Command(openRoutePointDialog);
        }

        private void openRoutePointDialog()
        {
            Navigation.PushAsync(new RoutePointV2Page(_vroute.Id, string.Empty));
        }

        public void startDialog()
        {
        }

        public string Name
        {
            get
            {
                return CommonResource.RouteCreated_RouteCreatedSuccessful.Replace("[routeName]", _vroute.Name);
            }
        }
    }
}
