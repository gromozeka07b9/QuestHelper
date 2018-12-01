using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using QuestHelper.Model;
using QuestHelper.View;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        public ICommand LoginCommand { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(TryLoginCommand);
        }
        void TryLoginCommand()
        {
            var pageCollections = new PagesCollection();
            MainPageMenuItem destinationPage = pageCollections.GetPageByPosition(0);
            Xamarin.Forms.MessagingCenter.Send<PageNavigationMessage>(new PageNavigationMessage() { DestinationPageDescription = destinationPage }, string.Empty);
        }
    }
}
