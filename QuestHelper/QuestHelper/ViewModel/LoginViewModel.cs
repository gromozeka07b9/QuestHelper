using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.OAuth;
using QuestHelper.View;
using QuestHelper.WS;
using Xamarin.Forms;
using static QuestHelper.WS.AccountApiRequest;

namespace QuestHelper.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _password;
        private string _passwordConfirm;
        private string _username;
        private string _apiUrl = "http://igosh.pro/api";
        private string _email;

        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        public ICommand LoginCommand { get; private set; }
        public ICommand GoToRegisterCommand { get; private set; }
        public ICommand RegisterCommand { get; private set; }
        public ICommand LoginWithGoogleCommand { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(TryLoginCommandAsync);
            GoToRegisterCommand = new Command(GoToRegisterCommandAsync);
            RegisterCommand = new Command(RegisterCommandAsync);
            LoginWithGoogleCommand = new Command(loginWithGoogleCommand);
        }

        private async void GoToRegisterCommandAsync()
        {
            await Navigation.PushModalAsync(new NavigationPage(new RegisterPage()));
        }
        private void loginWithGoogleCommand()
        {
            OAuthGoogleAuthenticator oAuth = new OAuthGoogleAuthenticator();
            oAuth.Login();
        }

        async void RegisterCommandAsync()
        {
            if ((!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password) && !string.IsNullOrEmpty(_email)))
            {
                if (_email.Contains("@") && _email.Contains("."))
                {
                    if (_password.Equals(_passwordConfirm))
                    {
                        using (UserDialogs.Instance.Loading("Регистрация...", null, null, true, MaskType.Black))
                        {
                            AccountApiRequest apiRequest = new AccountApiRequest(_apiUrl);
                            Analytics.TrackEvent("Register user started", new Dictionary<string, string> { { "Username", _username } });
                            TokenResponse authData = await apiRequest.RegisterNewUserAsync(_username, _password, _email);
                            if (!string.IsNullOrEmpty(authData?.Access_Token))
                            {
                                Analytics.TrackEvent("Register new user done", new Dictionary<string, string> { { "Username", _username } });
                                TokenStoreService tokenService = new TokenStoreService();
                                await tokenService.SetAuthDataAsync(authData.Access_Token, authData.UserId);
                                Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(), string.Empty);
                                var page = await Navigation.PopModalAsync();
                                ShowMainPage();
                            }
                            else
                            {
                                Analytics.TrackEvent("Register new user error", new Dictionary<string, string> { { "Username", _username } });
                                await Application.Current.MainPage.DisplayAlert("Внимание!", "Ошибка регистрации пользователя. Возможно, указанное имя или адрес e-mail уже заняты", "Ok");
                            }
                        }
                    }
                    else await Application.Current.MainPage.DisplayAlert("Внимание!", "Пожалуйста, введите одинаковые пароли", "Ok");
                }
                else await Application.Current.MainPage.DisplayAlert("Внимание!", "Пожалуйста, укажите корректный электронный адрес", "Ok");
            }
            else await Application.Current.MainPage.DisplayAlert("Внимание!", "Пожалуйста, заполните все поля", "Ok");
        }

        async void TryLoginCommandAsync()
        {
            if ((!string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password)))
            {
                using (UserDialogs.Instance.Loading("Авторизация...", null, null, true, MaskType.Black))
                {
                    string username = await DependencyService.Get<IUsernameService>().GetUsername();
                    AccountApiRequest apiRequest = new AccountApiRequest(_apiUrl);
                    Analytics.TrackEvent("GetToken started", new Dictionary<string, string> { { "Username", _username } });
                    TokenResponse authData = await apiRequest.GetTokenAsync(_username, _password);
                    if (!string.IsNullOrEmpty(authData?.Access_Token))
                    {
                        Analytics.TrackEvent("GetToken done", new Dictionary<string, string> { { "Username", _username } });
                        TokenStoreService tokenService = new TokenStoreService();
                        await tokenService.SetAuthDataAsync(authData.Access_Token, authData.UserId);
                        Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(), string.Empty);
                        ShowMainPage();
                    }
                    else
                    {
                        Analytics.TrackEvent("GetToken error", new Dictionary<string, string> { { "Username", _username } });
                        await Application.Current.MainPage.DisplayAlert("Внимание!", "Неправильный логин или пароль!", "Ok");
                    }
                }
            } else await Application.Current.MainPage.DisplayAlert("Внимание!", "Пожалуйста, заполните логин и пароль", "Ok");
        }
        /*async void DemoCommandAsync()
        {
            using (UserDialogs.Instance.Loading("Авторизация...", null, null, true, MaskType.Black))
            {
                string username = await DependencyService.Get<IUsernameService>().GetUsername();
                if (!string.IsNullOrEmpty(username))
                {
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
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Внимание!", "Не найдена учетная запись gmail для использования в демо-режиме", "Ok");
                }
            }
        }*/

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
        public string Email
        {
            set
            {
                if (_email != value)
                {
                    _email = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Email"));
                    }
                }
            }
            get
            {
                return _email;
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
        public string PasswordConfirm
        {
            set
            {
                if (_passwordConfirm != value)
                {
                    _passwordConfirm = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("PasswordConfirm"));
                    }
                }
            }
            get
            {
                return _passwordConfirm;
            }
        }

        public async void startDialogAsync()
        {
            string username = await DependencyService.Get<IUsernameService>().GetUsername();
            Email = username;
        }
    }
}
