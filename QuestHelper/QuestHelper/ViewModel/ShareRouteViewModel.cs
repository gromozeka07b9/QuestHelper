using Acr.UserDialogs;
using Newtonsoft.Json.Linq;
using QuestHelper.Model;
using QuestHelper.Model.WS;
using QuestHelper.Resources;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class ShareRouteViewModel : INotifyPropertyChanged
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private IEnumerable<ViewUserInfo> _usersForShare = new List<ViewUserInfo>();
        private List<ViewUserInfo> _usersFullList;
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        public ICommand SearchUsersCommand { get; private set; }
        public ICommand UpdateUsersCommand { get; private set; }
        private string _routeId = string.Empty;
        private bool _isRefreshing = false;
        public ShareRouteViewModel(string routeId)
        {
            SearchUsersCommand = new Command(searchUsersCommandAsync);
            UpdateUsersCommand = new Command(updateUsersCommand);
            _routeId = routeId;
            updateUsersCommand();
        }

        private void updateUsersCommand()
        {
            IsRefreshing = true;
            IsRefreshing = false;
        }

        private async void searchUsersCommandAsync(object text)
        {
            string searchTxt = text.ToString().Trim();
            if (!string.IsNullOrEmpty(searchTxt))
            {
                _usersFullList = await getUsersFromServerAsync(searchTxt);
                FilterUsersByTextAsync(text.ToString());
            }
        }

        public async void FilterUsersByTextAsync(string textForSearch)
        {
            IsRefreshing = true;
            _usersFullList = await getUsersFromServerAsync(textForSearch);
            IsRefreshing = false;
            string lowercaseTextForSearch = textForSearch.ToLower();
            if (!string.IsNullOrEmpty(lowercaseTextForSearch))
            {
                FoundedUsers = _usersFullList.Where(s => s.Name.ToLower().Contains(lowercaseTextForSearch) || s.Email.ToLower().Contains(lowercaseTextForSearch));
            }
            else
            {
                FoundedUsers = new List<ViewUserInfo>();
            }
        }

        private async System.Threading.Tasks.Task<List<ViewUserInfo>> getUsersFromServerAsync(string textForSearch)
        {
            TokenStoreService token = new TokenStoreService();
            string authToken = await token.GetAuthTokenAsync();
            var usersApi = new UsersApiRequest(_apiUrl, authToken);            
            return await usersApi.SearchUsers(textForSearch);
        }
        public IEnumerable<ViewUserInfo> FoundedUsers
        {
            set
            {
                if (_usersForShare != value)
                {
                    _usersForShare = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("FoundedUsers"));
                    PropertyChanged(this, new PropertyChangedEventArgs("NoContactWarningIsVisible"));
                }
            }
            get
            {
                return _usersForShare;
            }
        }
        public bool IsRefreshing
        {
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsRefreshing"));
                    }
                }
            }
            get
            {
                return _isRefreshing;
            }
        }
        public bool NoContactWarningIsVisible
        {
            get
            {
                return !FoundedUsers.Any();
            }
        }

        public ViewUserInfo SelectedFoundedUsersItem
        {
            set => inviteUserAsync(value);
        }

        private async void inviteUserAsync(ViewUserInfo user)
        {
            bool answerYesIsNo = await Application.Current.MainPage.DisplayAlert(CommonResource.CommonMsg_Warning, CommonResource.ShareRoute_AreYouSureAfterPublishYouCantDelete, CommonResource.CommonMsg_No, CommonResource.CommonMsg_Yes);
            if (!answerYesIsNo)//порядок кнопок - хардкод, и непонятно, почему именно такой
            {
                TokenStoreService token = new TokenStoreService();
                string authToken = await token.GetAuthTokenAsync();
                var routesApi = new RoutesApiRequest(_apiUrl, authToken);
                List<string> accessForUsersId = new List<string>();
                accessForUsersId.Add(user.UserId);

                ShareRequest shareRequest = new ShareRequest();
                shareRequest.RouteIdForShare = _routeId;
                shareRequest.UserId = accessForUsersId.ToArray();
                shareRequest.CanChangeRoute = true;
                JObject jsonRequestObject = JObject.FromObject(shareRequest);

                bool result = await routesApi.ShareRouteAsync(jsonRequestObject.ToString());
                string resultShareText = result ? CommonResource.ShareRoute_RouteSharedToSelectedUsers : CommonResource.ShareRoute_ErrorShareRoute;
                DependencyService.Get<IToastService>().ShortToast(resultShareText);

                await Navigation.PopAsync(true);
            }
        }
    }
}
