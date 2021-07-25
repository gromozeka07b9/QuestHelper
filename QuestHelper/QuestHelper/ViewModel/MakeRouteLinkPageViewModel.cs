using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.Resources;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Managers.Sync;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class MakeRouteLinkPageViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly string _routeId;
        private ViewRoute _vroute;
        private string _routeUrl = string.Empty;
        private string _captionText = string.Empty;
        private bool _makeLinkIsVisible = false;

        private const string _apiUrl = "https://igosh.pro/api";
        private const string _goshUrl = "https://igosh.pro";
        private string _authToken = string.Empty;
        private RoutesApiRequest _routesApi;

        private string _accessByLink = CommonResource.MakeRouteLink_PublicAccessEnabled;
        private string _makeLink = CommonResource.MakeRouteLink_PublicAccessDisabled;

        public ICommand UrlTappedCommand { get; private set; }
        public ICommand CopyUrlCommand { get; private set; }
        public ICommand MakeSharedLinkCommand { get; private set; }
        public ICommand ShareRouteCommand { get; private set; }
        public ICommand BackNavigationCommand { get; private set; }

        internal MakeRouteLinkPageViewModel(string routeId)
        {

            if (!string.IsNullOrEmpty(routeId))
            {
                _routeId = routeId;
                _vroute = new ViewRoute(routeId);
            }
            else throw new Exception("routeId is empty!");

            UrlTappedCommand = new Command(urlTappedCommandAsync);
            MakeSharedLinkCommand = new Command(makeSharedLinkCommandAsync);
            ShareRouteCommand = new Command(shareRouteCommand);
            BackNavigationCommand = new Command(backNavigationCommand);
            CopyUrlCommand = new Command(copyUrlCommand);
            CaptionText = _accessByLink;
        }

        private async void copyUrlCommand(object obj)
        {
            if (!string.IsNullOrEmpty(UrlPresentationText))
            {
                await Clipboard.SetTextAsync(UrlPresentationText);
                MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = CommonResource.MakeRouteLink_ReferenceCopiedToClipboard }, string.Empty);
            }
            else
            {
            }
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }

        private void shareRouteCommand()
        {
            var shareService = DependencyService.Get<ICommonShareService>();
            if (!string.IsNullOrEmpty(_routeUrl))
            {
                shareService.ShareWebLink(new Uri(_routeUrl), string.Empty);
            }
            else
            {
                HandleError.Process("shareRouteCommand", "Share route", new Exception("Empty URL"), false);
                Navigation.PopModalAsync();
            }
            
        }
        
        internal async Task<bool> UserCanShareAsync()
        {
            TokenStoreService tokenService = new TokenStoreService();
            string userId = await tokenService.GetUserIdAsync();
            return userId.Equals(_vroute.CreatorId);
        }

        private async void makeSharedLinkCommandAsync(object obj)
        {
            Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "publish" } });
            bool answerYesIsNo = await Application.Current.MainPage.DisplayAlert(CommonResource.CommonMsg_Warning, CommonResource.ShareRoute_AreYouSureToPublishRoute, CommonResource.CommonMsg_No, CommonResource.CommonMsg_Yes);
            if (!answerYesIsNo) //порядок кнопок - хардкод, и непонятно, почему именно такой
            {
                await publishToFeed();
                await makeLink();
            }
        }

        private async Task makeLink()
        {
            TokenStoreService token = new TokenStoreService();
            string _authToken = await token.GetAuthTokenAsync();
            _routesApi = new RoutesApiRequest(_apiUrl, _authToken);
            var makeResult = await _routesApi.CreateShortLinkIdAsync(_routeId);
            if (makeResult)
            {
                updateUrlCommandAsync();
                MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = CommonResource.MakeRouteLink_PublicReferenceCreated }, string.Empty);
            }
            else
            {
                MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = CommonResource.MakeRouteLink_ErrorWhileCreatedReference }, string.Empty);
            }
        }

        private async Task publishToFeed()
        {
            if (await UserCanShareAsync())
            {
                _vroute.IsPublished = true;
                _vroute.Version++;
                _vroute.ObjVerHash = string.Empty;
                _vroute.ServerSynced = false;
                _vroute.Save();
                await startSyncRouteAsync(_vroute.Id);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(CommonResource.CommonMsg_Warning,
                    CommonResource.ShareRoute_PublishOnlyYourRoutes, CommonResource.CommonMsg_Ok);
            }
        }

        private async Task startSyncRouteAsync(string routeId)
        {
            SyncServer syncSrv = new SyncServer();
            await syncSrv.Sync(routeId, false).ContinueWith(result =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (!result.Result)
                    {
                        UserDialogs.Instance.Alert(CommonResource.Sync_Error, CommonResource.CommonMsg_Warning, "Ok");
                    }
                    else
                    {
                        UserDialogs.Instance.Alert(CommonResource.ShareRoute_Published, "", "Ok");
                    }
                });

            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private async void urlTappedCommandAsync()
        {
            try
            {
                await Browser.OpenAsync(UrlPresentationText);
            }
            catch (Exception e)
            {
                HandleError.Process("urlTappedCommand", "Open in browser", e, false);
            }
        }

        private async void updateUrlCommandAsync()
        {
            TokenStoreService token = new TokenStoreService();
            string _authToken = await token.GetAuthTokenAsync();
            _routesApi = new RoutesApiRequest(_apiUrl, _authToken);
            var shortLinkRouteId = await _routesApi.GetShortLinkId(_routeId);
            if (!string.IsNullOrEmpty(shortLinkRouteId))
            {
                MakeLinkIsVisible = false;
                UrlPresentationText = $"{_goshUrl}/routetimeline/{_routeId}";
                CaptionText = _accessByLink;
            }
            else
            {
                MakeLinkIsVisible = true;
                UrlPresentationText = "";
                CaptionText = _makeLink;
            }
        }

        public void StartDialog()
        {
            updateUrlCommandAsync();
        }

        public void CloseDialog()
        {
        }

        public string UrlPresentationText
        {
            get { return _routeUrl; }
            set
            {
                if (!_routeUrl.Equals(value))
                {
                    _routeUrl = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("UrlPresentationText"));
                }
            }
        }
        public string CaptionText
        {
            get { return _captionText; }
            set
            {
                if (!_captionText.Equals(value))
                {
                    _captionText = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CaptionText"));
                }
            }
        }
        public bool MakeLinkIsVisible
        {
            get { return _makeLinkIsVisible; }
            set
            {
                if (_makeLinkIsVisible != value)
                {
                    _makeLinkIsVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MakeLinkIsVisible"));
                }
            }
        }
    }
}
