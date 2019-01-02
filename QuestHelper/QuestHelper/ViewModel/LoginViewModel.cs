using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.View;
using QuestHelper.WS;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _password;
        private string _username;
        private string _apiUrl = "http://igosh.pro/api";

        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand DemoCommand { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(TryLoginCommandAsync);
            DemoCommand = new Command(DemoCommandAsync);
        }
        async void TryLoginCommandAsync()
        {
            string username = await DependencyService.Get<IUsernameService>().GetUsername();
            AccountApiRequest apiRequest = new AccountApiRequest(_apiUrl);
            Analytics.TrackEvent("GetToken started", new Dictionary<string, string> { { "Username", _username } });
            var authToken = await apiRequest.GetTokenAsync(_username, _password);
            if (!string.IsNullOrEmpty(authToken))
            {
                Analytics.TrackEvent("GetToken done", new Dictionary<string, string> { { "Username", _username } });
                TokenStoreService tokenService = new TokenStoreService();
                await tokenService.SetAuthTokenAsync(authToken);
                Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(), string.Empty);
                ShowMainPage();
            }
            else
            {
                Analytics.TrackEvent("GetToken error", new Dictionary<string, string> { { "Username", _username } });
                await Application.Current.MainPage.DisplayAlert("Внимание!", "Неправильный логин или пароль!", "Ok");
            }

        }
        async void DemoCommandAsync()
        {
            string username = await DependencyService.Get<IUsernameService>().GetUsername();
            DependencyService.Get<IToastService>().LongToast($"Username:{username}");
            Analytics.TrackEvent("GetToken Demo started", new Dictionary<string, string> { { "Username", username } });
            AccountApiRequest apiRequest = new AccountApiRequest(_apiUrl);
            //Для демо режима пароль такой же
            var authToken = await apiRequest.GetTokenAsync(username, username, true);
            if (!string.IsNullOrEmpty(authToken))
            {
                Analytics.TrackEvent("GetToken Demo done", new Dictionary<string, string> { { "Username", username } });
                TokenStoreService tokenService = new TokenStoreService();
                await tokenService.SetAuthTokenAsync(authToken);
                Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(), string.Empty);
                ShowMainPage();
            }
            else
            {
                Analytics.TrackEvent("GetToken Demo error", new Dictionary<string, string> { { "Username", username } });
                await Application.Current.MainPage.DisplayAlert("Внимание!", "Неправильный логин или пароль!", "Ok");
            }

        }

        private static void ShowMainPage()
        {
            var pageCollections = new PagesCollection();
            MainPageMenuItem destinationPage = pageCollections.GetPageByPosition(0);
            Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(
                new PageNavigationMessage() {DestinationPageDescription = destinationPage}, string.Empty);
            var toolbarService = DependencyService.Get<IToolbarService>();
            if (toolbarService.ToolbarIsHidden())
            {
                toolbarService.SetVisibilityToolbar(true);
            }
        }

        public string Username
        {
            set
            {
                if (_username != value)
                {
                    _username = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Username"));
                    }
                }
            }
            get
            {
                return _username;
            }
        }
        public string Password
        {
            set
            {
                if (_password != value)
                {
                    _password = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Password"));
                    }
                }
            }
            get
            {
                return _password;
            }
        }
    }
}
