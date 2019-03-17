using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using QuestHelper.Managers;
using QuestHelper.Model.Messages;
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

        public SplashWizardViewModel()
        {
            NextPageTourCommand = new Command(nextPageTourCommand);
            SkipTourCommand = new Command(skipTourCommand);
        }

        private async void skipTourCommand(object obj)
        {
            ParameterManager par = new ParameterManager();
            par.Set("NeedShowOnboarding", "0");
            await Navigation.PopModalAsync();
        }

        private void nextPageTourCommand(object obj)
        {
        }
    }
}
