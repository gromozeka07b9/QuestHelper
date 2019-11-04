using Acr.UserDialogs;
using Newtonsoft.Json.Linq;
using QuestHelper.Model;
using QuestHelper.Model.WS;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using QuestHelper.View;
using Xamarin.Forms;
using QuestHelper.Model.Messages;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Resources;

namespace QuestHelper.ViewModel
{
    public class ShareRoutesServicesViewModel : INotifyPropertyChanged, IDialogEvents
    {
        private const string _apiUrl = "http://igosh.pro/api";
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isUserCanMakeLink = false;

        public ICommand TapAddUserCommand { get; private set; }
        public ICommand TapPublishAlbumCommand { get; private set; }
        public ICommand TapMakeReferenceCommand { get; private set; }
        public ICommand TapOtherCommand { get; private set; }
        public ICommand TapTelegramCommand { get; private set; }
        public ICommand TapInstagramCommand { get; private set; }
        public ICommand TapFacebookCommand { get; private set; }
        public ICommand TapViberCommand { get; private set; }
        public ICommand TapWhatsappCommand { get; private set; }
        public ICommand TapGMailCommand { get; private set; }

        public INavigation Navigation { get; set; }
        private readonly string _routeId;
        private ViewRoute _vroute;
        private IApplicationInstalledService _appInstalledService;

        private readonly Dictionary<string, string> _namesForShareApps = new Dictionary<string, string>()
        {
            { "instagram", "com.instagram.android" },
            { "facebook", "com.facebook.katana"},
            { "telegram", "org.telegram.messenger" },
            { "viber", "com.viber.voip" },
            { "whatsapp", "com.whatsapp" },
            { "gmail", "com.google.android.gm" },
            { "vk", "com.vk" },
        };
        private readonly Dictionary<string, string> _actionsForPopupMenu = new Dictionary<string, string>()
        {
            { "onlyPhotos", CommonResource.ShareRoutes_ExportPhotos },
            { "onlyTexts", CommonResource.ShareRoutes_ExportDescriptions},
            { "cancel", CommonResource.CommonMsg_Cancel }
        };
        public ShareRoutesServicesViewModel(string routeId)
        {
            if (!string.IsNullOrEmpty(routeId))
            {
                _routeId = routeId;
                _vroute = new ViewRoute(routeId);
            } else throw new Exception("routeId is empty!");

            _appInstalledService = DependencyService.Get<IApplicationInstalledService>();

            TapAddUserCommand = new Command(tapAddUserCommandAsync);
            TapPublishAlbumCommand = new Command(tapPublishAlbumCommand);
            TapMakeReferenceCommand = new Command(tapMakeReferenceCommand);
            TapOtherCommand = new Command(tapOtherCommand);
            TapTelegramCommand = new Command(tapTelegramCommand);
            TapInstagramCommand = new Command(tapInstagramCommand);
            TapFacebookCommand = new Command(tapFacebookCommand);
            TapViberCommand = new Command(tapViberCommand);
            TapWhatsappCommand = new Command(tapWhatsappCommand);
            TapGMailCommand = new Command(tapGMailCommand);
            _routeId = routeId;
        }

        private void tapInstagramCommand(object obj)
        {
            Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "instagram" } });
            var shareService = DependencyService.Get<IInstagramShareService>();
            shareService.Share(_vroute, _namesForShareApps["instagram"]);
        }

        private async void tapGMailCommand(object obj)
        {
            Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "gmail" } });
            var shareService = DependencyService.Get<ICommonShareService>();
            shareService.Share(_vroute, _namesForShareApps["gmail"]);
            await Navigation.PopAsync(false);
        }

        private void tapOtherCommand(object obj)
        {
            Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "other" } });
            var shareService = DependencyService.Get<ICommonShareService>();
            shareService.Share(_vroute, string.Empty);
        }

        private void tapMakeReferenceCommand(object obj)
        {
            Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "makeRef" } });
            var page = new MakeRouteLinkPage(_vroute.RouteId);
            Navigation.PushAsync(page);
        }

        private async void tapPublishAlbumCommand(object obj)
        {
            Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "publish" } });
            bool answerYesIsNo = await Application.Current.MainPage.DisplayAlert(CommonResource.CommonMsg_Warning, CommonResource.ShareRoute_AreYouSureToPublishRoute, CommonResource.CommonMsg_No, CommonResource.CommonMsg_Yes);
            if (!answerYesIsNo) //порядок кнопок - хардкод, и непонятно, почему именно такой
            {
                if (await UserCanShareAsync())
                {
                    _vroute.IsPublished = true;
                    _vroute.Version++;
                    _vroute.ObjVerHash = string.Empty;
                    _vroute.Save();
                    await Application.Current.MainPage.DisplayAlert(CommonResource.CommonMsg_Warning, CommonResource.ShareRoute_RouteWillBePublishAfterSync, CommonResource.CommonMsg_Ok);
                    Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage() { RouteId = _vroute.Id, ShowErrorMessageIfExist = false }, string.Empty);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(CommonResource.CommonMsg_Warning, CommonResource.ShareRoute_PublishOnlyYourRoutes, CommonResource.CommonMsg_Ok);
                }
            }
        }
        internal async Task<bool> UserCanShareAsync()
        {
            TokenStoreService tokenService = new TokenStoreService();
            string userId = await tokenService.GetUserIdAsync();
            return userId.Equals(_vroute.CreatorId);
        }

        private async void tapAddUserCommandAsync(object obj)
        {
            Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "addUser" } });
            var shareRoutePage = new ShareRoutePage(_vroute.RouteId);
            await Navigation.PushAsync(shareRoutePage, true);
        }
        private async void tapTelegramCommand(object obj)
        {
            if (_vroute.IsHaveAnyPhotos)
            {
                var result = await Application.Current.MainPage.DisplayActionSheet(null, _actionsForPopupMenu["cancel"], null, new[] { _actionsForPopupMenu["onlyPhotos"], _actionsForPopupMenu["onlyTexts"] });
                if (result != null)
                {
                    if (result.Equals(_actionsForPopupMenu["onlyPhotos"]))
                    {
                        Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "telegramPhotos" } });
                        var shareService = DependencyService.Get<ITelegramShareService>();
                        shareService.Share(_vroute, _namesForShareApps["telegram"]);
                        await Navigation.PopAsync(false);
                    }
                    else if (result.Equals(_actionsForPopupMenu["onlyTexts"]))
                    {
                        Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "telegramDescription" } });
                        var shareService = DependencyService.Get<ITelegramShareService>();
                        shareService.ShareRouteOnlyPointsDescription(_vroute, _namesForShareApps["telegram"]);
                        await Navigation.PopAsync(false);
                    }
                }
            }
            else
            {
                Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "telegramDescription" } });
                var shareService = DependencyService.Get<ITelegramShareService>();
                shareService.ShareRouteOnlyPointsDescription(_vroute, _namesForShareApps["telegram"]);
                await Navigation.PopAsync(false);
            }
        }
        private async void tapFacebookCommand(object obj)
        {
            Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "facebook" } });
            var shareService = DependencyService.Get<IFacebookShareService>();
            shareService.Share(_vroute, _namesForShareApps["facebook"]);
            await Navigation.PopAsync(false);
        }
        private async void tapViberCommand(object obj)
        {
            Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "viber" } });
            var shareService = DependencyService.Get<IViberShareService>();
            shareService.Share(_vroute, _namesForShareApps["viber"]);
            await Navigation.PopAsync(false);
        }
        private async void tapWhatsappCommand(object obj)
        {
            Analytics.TrackEvent("Share service", new Dictionary<string, string> { { "TypeShare", "whatsapp" } });
            var shareService = DependencyService.Get<IWhatsappShareService>();
            shareService.Share(_vroute, _namesForShareApps["whatsapp"]);
            await Navigation.PopAsync(false);
        }

        public async void StartDialog()
        {
            TokenStoreService tokenService = new TokenStoreService();
            string currentUserId = await tokenService.GetUserIdAsync();
            IsUserCanMakeLink = _vroute.CreatorId.Equals(currentUserId);
        }

        public void CloseDialog()
        {
        }

        public bool IsUserCanMakeLink
        {
            get
            {
                return _isUserCanMakeLink;
            }
            set
            {
                if (!_isUserCanMakeLink.Equals(value))
                {
                    _isUserCanMakeLink = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsUserCanMakeLink"));
                }
            }
        }

        public bool IsAppInstagramInstalled
        {
            get
            {
                return _appInstalledService.AppInstalled(_namesForShareApps["instagram"]);
            }
        }
        public bool IsAppFacebookInstalled
        {
            get
            {
                return _appInstalledService.AppInstalled(_namesForShareApps["facebook"]);
            }
        }

        public bool IsAppTelegramInstalled
        {
            get
            {
                return _appInstalledService.AppInstalled(_namesForShareApps["telegram"]);
            }
        }

        public bool IsAppViberInstalled
        {
            get
            {
                return _appInstalledService.AppInstalled(_namesForShareApps["viber"]);
            }
        }
        public bool IsAppWhatsappInstalled
        {
            get
            {
                return _appInstalledService.AppInstalled(_namesForShareApps["whatsapp"]);
            }
        }
        public bool IsAppVkInstalled
        {
            get
            {
                return _appInstalledService.AppInstalled(_namesForShareApps["vk"]);
            }
        }
        public bool IsAppGMailInstalled
        {
            get
            {
                return _appInstalledService.AppInstalled(_namesForShareApps["gmail"]);
            }
        }

    }
}
