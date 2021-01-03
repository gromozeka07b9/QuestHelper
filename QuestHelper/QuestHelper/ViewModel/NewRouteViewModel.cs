using QuestHelper.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using QuestHelper.Model;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Resources;

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
            _vroute = new ViewRoute(string.Empty);
            _isFirstRoute = isFirstRoute;
            ShowNewRouteDialogCommand = new Command(showNewRouteData);
            OpenRoutePointDialogCommand = new Command(openRoutePointDialogAsync);
        }

        private async void openRoutePointDialogAsync()
        {
            if(string.IsNullOrEmpty(_vroute.Name))
            {
                await App.Current.MainPage.DisplayAlert(CommonResource.CommonMsg_Warning, CommonResource.NewRoute_NeedToFillNameRoute, "Ok");
            } else
            {
                Analytics.TrackEvent("Route created", new Dictionary<string, string> { { "Route", _vroute.Name } });
                TokenStoreService tokenService = new TokenStoreService();
                _vroute.CreatorId = await tokenService.GetUserIdAsync();
                if (_vroute.Save())
                {
                    //MessagingCenter.Send<PageNavigationMessage>(new PageNavigationMessage() { PageToOpen = MainPages.Private }, string.Empty);
                    await Navigation.PushAsync(new RouteCreatedPage(_vroute.Id));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SplashStartScreenIsVisible"));
                }
            }
            get
            {
                return _splashStartScreenIsVisible;
            }
        }
    }
}
