using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
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

        private const string _apiUrl = "http://igosh.pro/api";
        private const string _goshUrl = "http://igosh.pro";
        private string _authToken = string.Empty;
        private RoutesApiRequest _routesApi;

        private const string _accessByLink = "Общий доступ к маршруту включен";
        private const string _makeLink = "Общий доступ по ссылке пока отключен";

        public ICommand UrlTappedCommand { get; private set; }
        public ICommand MakeSharedLinkCommand { get; private set; }
        public ICommand ShareRouteCommand { get; private set; }

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

            CaptionText = _accessByLink;
        }

        private void shareRouteCommand()
        {
            var shareService = DependencyService.Get<ICommonShareService>();
            shareService.ShareWebLink(new Uri(_routeUrl), string.Empty);
        }

        private async void makeSharedLinkCommandAsync()
        {
            TokenStoreService token = new TokenStoreService();
            string _authToken = await token.GetAuthTokenAsync();
            _routesApi = new RoutesApiRequest(_apiUrl, _authToken);
            var makeResult = await _routesApi.CreateShortLinkIdAsync(_routeId);
            if (makeResult)
            {
                updateUrlCommandAsync();
                MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = "Создана ссылка для общего доступа к маршруту" }, string.Empty);
            }
            else
            {
                MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = "Ошибка включения общего доступа к маршруту. Проверьте ваше подключение к сети." }, string.Empty);
            }
        }

        private async void urlTappedCommandAsync()
        {
            if (!string.IsNullOrEmpty(UrlPresentationText))
            {
                await Clipboard.SetTextAsync(UrlPresentationText);
                MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = "Ссылка скопирована в буфер обмена" }, string.Empty);
            }
            else
            {
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
                UrlPresentationText = $"{_goshUrl}/gallery/{shortLinkRouteId}";
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
