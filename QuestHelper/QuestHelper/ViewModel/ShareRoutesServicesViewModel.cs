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
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class ShareRoutesServicesViewModel : INotifyPropertyChanged
    {
        private const string _apiUrl = "http://igosh.pro/api";
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand TapCommand { get; private set; }
        public INavigation Navigation { get; set; }
        /*private IEnumerable<ViewUserInfo> _usersForShare;
        private List<ViewUserInfo> _usersFullList;
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        public ICommand SearchUsersCommand { get; private set; }
        public ICommand UpdateUsersCommand { get; private set; }
        private string _routeId = string.Empty;
        private bool _isRefreshing = false;*/
        public ShareRoutesServicesViewModel(string routeId)
        {
            TapCommand = new Command(tapCommand);
            //UpdateUsersCommand = new Command(updateUsersCommand);
            //_routeId = routeId;
        }

        private void tapCommand(object obj)
        {
        }
    }
}
