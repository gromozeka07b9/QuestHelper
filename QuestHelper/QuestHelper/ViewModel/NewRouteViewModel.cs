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
using Microsoft.AppCenter.Analytics;

namespace QuestHelper.ViewModel
{
    public class NewRouteViewModel : INotifyPropertyChanged
    {
        private bool _splashStartScreenIsVisible;
        private ViewRoute _vroute;

        private bool _isFirstRoute;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand ShowNewRouteDialogCommand { get; private set; }
        public ICommand OpenRoutePointDialogCommand { get; private set; }

        public NewRouteViewModel(bool isFirstRoute)
        {
            _vroute = new ViewRoute();
            _vroute.CreateDate = DateTimeOffset.Now;
            _vroute.Version = 1;
            _isFirstRoute = isFirstRoute;
            ShowNewRouteDialogCommand = new Command(showNewRouteData);
            OpenRoutePointDialogCommand = new Command(openRoutePointDialog);
        }

        private void openRoutePointDialog()
        {
            if(string.IsNullOrEmpty(_vroute.Name))
            {
                App.Current.MainPage.DisplayAlert("Внимание!", "Необходимо заполнить название маршрута", "Ok");
            } else
            {
                Analytics.TrackEvent("Route created", new Dictionary<string, string> { { "Route", _vroute.Name } });
                if (_vroute.Save())
                {
                    Navigation.PushAsync(new RouteCreatedPage(_vroute.Id));
                }
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
                if (_vroute.Name != value)
                {
                    _vroute.Name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _vroute.Name;
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
