using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using QuestHelper.Model;
using QuestHelper.View;
using QuestHelper.WS;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _password;
        private string _username;

        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        public ICommand LoginCommand { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(TryLoginCommandAsync);
        }
        async void TryLoginCommandAsync()
        {
            string apiUrl = "http://igosh.pro/api";
            AccountApiRequest apiRequest = new AccountApiRequest(apiUrl);
            var authToken = await apiRequest.GetTokenAsync(_username, _password);
            if (!string.IsNullOrEmpty(authToken))
            {
                TokenStoreService tokenService = new TokenStoreService();
                tokenService.SetAuthToken(authToken);
                var pageCollections = new PagesCollection();
                MainPageMenuItem destinationPage = pageCollections.GetPageByPosition(0);
                Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(new PageNavigationMessage() { DestinationPageDescription = destinationPage }, string.Empty);
                var toolbarService = DependencyService.Get<IToolbarService>();
                if (toolbarService.ToolbarIsHidden())
                {
                    toolbarService.SetVisibilityToolbar(true);
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Внимание!", "Неправильный логин или пароль!", "Отмена");
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
