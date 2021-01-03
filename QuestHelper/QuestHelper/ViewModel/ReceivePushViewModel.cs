using System.ComponentModel;
using System.Windows.Input;
using QuestHelper.Resources;
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MessageTitle"));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MessageBody"));
                }
            }
            get
            {
                return _messageBody;
            }
        }

        public void StartDialog()
        {
            MessageTitle = CommonResource.ReceivePush_YouHaveNewRoutes;
            MessageBody = "";
        }
        public void StopDialog()
        {
        }
    }
}
