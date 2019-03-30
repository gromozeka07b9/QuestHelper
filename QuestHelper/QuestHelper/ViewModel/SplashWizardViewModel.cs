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

        internal void SetStatusNoNeedShowOnboarding()
        {
            ParameterManager par = new ParameterManager();
            par.Set("NeedShowOnboarding", "0");
        }
        private void skipTourCommand()
        {
            SetStatusNoNeedShowOnboarding();
            Navigation.PopModalAsync();
        }

        private void nextPageTourCommand()
        {
        }
    }
}
