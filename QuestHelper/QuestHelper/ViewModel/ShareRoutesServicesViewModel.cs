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
    public class ShareRoutesServicesViewModel : INotifyPropertyChanged
    {
        private const string _apiUrl = "http://igosh.pro/api";
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand TapAddUserCommand { get; private set; }
        public ICommand TapPublishAlbumCommand { get; private set; }
        public ICommand TapMakeReferenceCommand { get; private set; }
        public ICommand TapOtherCommand { get; private set; }

        public INavigation Navigation { get; set; }
        private readonly string _routeId;
        public ShareRoutesServicesViewModel(string routeId)
        {
            TapAddUserCommand = new Command(tapAddUserCommandAsync);
            TapPublishAlbumCommand = new Command(tapPublishAlbumCommand);
            TapMakeReferenceCommand = new Command(tapMakeReferenceCommand);
            TapOtherCommand = new Command(tapOtherCommand);
            _routeId = routeId;
        }

        private void tapOtherCommand(object obj)
        {
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
    }
}
