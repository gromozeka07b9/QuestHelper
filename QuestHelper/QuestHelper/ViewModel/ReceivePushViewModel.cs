using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class ReceivePushViewModel : INotifyPropertyChanged
    {
        private string _messageTitle;
        private string _messageBody;

        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        //public ICommand GotoRouteCommand { get; private set; }
        public ICommand OkCommand { get; private set; }

        public ReceivePushViewModel()
        {
            //GotoRouteCommand = new Command(gotoRouteCommandAsync);
            OkCommand = new Command(okCommandAsync);
        }

        private async void okCommandAsync()
        {
            await Navigation.PopAsync();
        }

        public string MessageTitle
        {
            set
            {
                if (_messageTitle != value)
                {
                    _messageTitle = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("MessageTitle"));
                    }
                }
            }
            get
            {
                return _messageTitle;
            }
        }
        public string MessageBody
        {
            set
            {
                if (_messageBody != value)
                {
                    _messageBody = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("MessageBody"));
                    }
                }
            }
            get
            {
                return _messageBody;
            }
        }

        public void StartDialog()
        {
            var toolbarService = DependencyService.Get<IToolbarService>();
            if (!toolbarService.ToolbarIsHidden())
            {
                toolbarService.SetVisibilityToolbar(false);
            }

            MessageTitle = "Вам доступны новые маршруты";
            MessageBody = "";
        }
        public void StopDialog()
        {
        }
    }
}
