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
using System.Windows.Input;
using QuestHelper.View;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class ShareRoutesServicesViewModel : INotifyPropertyChanged, IDialogEvents
    {
        private const string _apiUrl = "http://igosh.pro/api";
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand TapAddUserCommand { get; private set; }
        public ICommand TapPublishAlbumCommand { get; private set; }
        public ICommand TapMakeReferenceCommand { get; private set; }
        public ICommand TapOtherCommand { get; private set; }
        public ICommand TapTelegramCommand { get; private set; }
        public ICommand TapFacebookCommand { get; private set; }
        public ICommand TapViberCommand { get; private set; }
        public ICommand TapWhatsappCommand { get; private set; }

        public INavigation Navigation { get; set; }
        private readonly string _routeId;

        private IApplicationInstalledService _appInstalledService;

        private readonly Dictionary<string, string> _namesForShareApps = new Dictionary<string, string>()
        {
            { "instagram", "com.android.instagram" },
            { "facebook", "com.facebook.katana"},
            { "telegram", "org.telegram.messenger" },
            { "viber", "com.viber.voip" },
            { "whatsapp", "com.whatsapp" },
            { "vk", "com.vk" },
        };
        public ShareRoutesServicesViewModel(string routeId)
        {
            _appInstalledService = DependencyService.Get<IApplicationInstalledService>();

            TapAddUserCommand = new Command(tapAddUserCommandAsync);
            TapPublishAlbumCommand = new Command(tapPublishAlbumCommand);
            TapMakeReferenceCommand = new Command(tapMakeReferenceCommand);
            TapOtherCommand = new Command(tapOtherCommand);
            TapTelegramCommand = new Command(tapTelegramCommand);
            TapFacebookCommand = new Command(tapFacebookCommand);
            TapViberCommand = new Command(tapViberCommand);
            TapWhatsappCommand = new Command(tapWhatsappCommand);
            _routeId = routeId;
        }

        private void tapOtherCommand(object obj)
        {
            var shareService = DependencyService.Get<ICommonShareService>();
            shareService.Share(new ViewRoute(_routeId), string.Empty);
        }

        private void tapMakeReferenceCommand(object obj)
        {
        }

        private void tapPublishAlbumCommand(object obj)
        {
        }

        private async void tapAddUserCommandAsync(object obj)
        {
            var shareRoutePage = new ShareRoutePage(_routeId);
            await Navigation.PushAsync(shareRoutePage, true);
        }
        private async void tapTelegramCommand(object obj)
        {
            var shareService = DependencyService.Get<ITelegramShareService>();
            shareService.Share(new ViewRoute(_routeId), _namesForShareApps["telegram"]);
        }
        private async void tapFacebookCommand(object obj)
        {
            var shareService = DependencyService.Get<IFacebookShareService>();
            shareService.Share(new ViewRoute(_routeId), _namesForShareApps["facebook"]);
        }
        private async void tapViberCommand(object obj)
        {
            var shareService = DependencyService.Get<IViberShareService>();
            shareService.Share(new ViewRoute(_routeId), _namesForShareApps["viber"]);
        }
        private async void tapWhatsappCommand(object obj)
        {
            var shareService = DependencyService.Get<IWhatsappShareService>();
            shareService.Share(new ViewRoute(_routeId), _namesForShareApps["whatsapp"]);
        }

        public void StartDialog()
        {
        }

        public void CloseDialog()
        {
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

    }
}
