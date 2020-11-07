using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }

        public SettingsViewModel()
        {
            BackNavigationCommand = new Command(backNavigationCommand);
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }

        public void StartDialog()
        {

        }

        public void CloseDialog()
        {

        }
    }
}
