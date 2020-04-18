using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using QuestHelper.Managers;
using QuestHelper.Model.Messages;
using QuestHelper.Resources;
using QuestHelper.View;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class SplashWizardViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand NextPageTourCommand { get; private set; }
        public ICommand SkipTourCommand { get; private set; }
        public ICommand RequestPermissionLocationCommand { get; private set; }

        
        public SplashWizardViewModel()
        {
            NextPageTourCommand = new Command(nextPageTourCommand);
            SkipTourCommand = new Command(skipTourCommand);
            RequestPermissionLocationCommand = new Command(requestPermissionLocationCommand);
        }

        internal void SetStatusNoNeedShowOnboarding()
        {
            ParameterManager par = new ParameterManager();
            par.Set("NeedShowOnboarding", "0");
        }

        private void skipTourCommand()
        {
            SetStatusNoNeedShowOnboarding();

            requestPermissionLocationCommand(new object());
        }

        private void nextPageTourCommand()
        {
        }

        private async void requestPermissionLocationCommand(object obj)
        {
            SetStatusNoNeedShowOnboarding();
            PermissionManager permissions = new PermissionManager();
            await permissions.PermissionLocationGrantedAsync(CommonResource.Permission_Position);
            App.Current.MainPage = new View.MainPage();
        }
    }
}
