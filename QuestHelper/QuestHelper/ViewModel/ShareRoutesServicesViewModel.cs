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
        public ICommand TapGMailCommand { get; private set; }

        public INavigation Navigation { get; set; }
        private readonly string _routeId;
        private ViewRoute _vroute;
        private IApplicationInstalledService _appInstalledService;

        private readonly Dictionary<string, string> _namesForShareApps = new Dictionary<string, string>()
        {
            { "instagram", "com.android.instagram" },
            { "facebook", "com.facebook.katana"},
            { "telegram", "org.telegram.messenger" },
            { "viber", "com.viber.voip" },
            { "whatsapp", "com.whatsapp" },
            { "gmail", "com.google.android.gm" },
            { "vk", "com.vk" },
        };
        private readonly Dictionary<string, string> _actionsForPopupMenu = new Dictionary<string, string>()
        {
            { "onlyPhotos", "Выгрузить фотографии" },
            { "onlyTexts", "Выгрузить описания"},
            { "cancel", "Отмена" }
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
            TapFacebookCommand = new Command(tapFacebookCommand);
            TapViberCommand = new Command(tapViberCommand);
            TapWhatsappCommand = new Command(tapWhatsappCommand);
            TapGMailCommand = new Command(tapGMailCommand);
            _routeId = routeId;
        }

        private async void tapGMailCommand(object obj)
        {
            var shareService = DependencyService.Get<ICommonShareService>();
            shareService.Share(_vroute, _namesForShareApps["gmail"]);
            await Navigation.PopAsync(false);
        }

        private void tapOtherCommand(object obj)
        {
            var shareService = DependencyService.Get<ICommonShareService>();
            shareService.Share(_vroute, string.Empty);
        }

        private void tapMakeReferenceCommand(object obj)
        {
        }

        private async void tapPublishAlbumCommand(object obj)
        {
            bool answerYesIsNo = await Application.Current.MainPage.DisplayAlert("Внимание", "После публикации маршрут будет доступен всем пользователям в альбоме. Вы уверены?", "Нет", "Да");
            if (!answerYesIsNo) //порядок кнопок - хардкод, и непонятно, почему именно такой
            {
                if (await UserCanShareAsync())
                {
                    _vroute.IsPublished = true;
                    _vroute.Version++;
                    _vroute.Save();
                    await Application.Current.MainPage.DisplayAlert("Внимание!", "После синхронизации маршрут будет опубликован", "Ok");
                    Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage() { ShowErrorMessageIfExist = false }, string.Empty);
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Внимание!", "Публиковать можно только созданные вами маршруты", "Ok");
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
                        var shareService = DependencyService.Get<ITelegramShareService>();
                        shareService.Share(_vroute, _namesForShareApps["telegram"]);
                        await Navigation.PopAsync(false);
                    }
                    else if (result.Equals(_actionsForPopupMenu["onlyTexts"]))
                    {
                        var shareService = DependencyService.Get<ITelegramShareService>();
                        shareService.ShareRouteOnlyPointsDescription(_vroute, _namesForShareApps["telegram"]);
                        await Navigation.PopAsync(false);
                    }
                }
            }
            else
            {
                var shareService = DependencyService.Get<ITelegramShareService>();
                shareService.ShareRouteOnlyPointsDescription(_vroute, _namesForShareApps["telegram"]);
                await Navigation.PopAsync(false);
            }
        }
        private async void tapFacebookCommand(object obj)
        {
            var shareService = DependencyService.Get<IFacebookShareService>();
            shareService.Share(_vroute, _namesForShareApps["facebook"]);
            await Navigation.PopAsync(false);
        }
        private async void tapViberCommand(object obj)
        {
            var shareService = DependencyService.Get<IViberShareService>();
            shareService.Share(_vroute, _namesForShareApps["viber"]);
            await Navigation.PopAsync(false);
        }
        private async void tapWhatsappCommand(object obj)
        {
            var shareService = DependencyService.Get<IWhatsappShareService>();
            shareService.Share(_vroute, _namesForShareApps["whatsapp"]);
            await Navigation.PopAsync(false);
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
        public bool IsAppGMailInstalled
        {
            get
            {
                return _appInstalledService.AppInstalled(_namesForShareApps["gmail"]);
            }
        }

    }
}
