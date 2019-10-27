using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using QuestHelper.ViewModel;
using Xamarin.Forms;

namespace QuestHelper.View
{
    public partial class MasterMainPage : ContentPage
    {
        public ListView ListView;
        private MasterMainPageViewModel vm = new MasterMainPageViewModel();

        public MasterMainPage()
        {
            InitializeComponent();
            BindingContext = vm;
            ListView = MenuItemsListView;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void MasterMainPage_OnAppearing(object sender, EventArgs e)
        {
            vm.StartDialog();
        }

        private void MasterMainPage_OnDisappearing(object sender, EventArgs e)
        {
            vm.CloseDialog();
        }
    }

    class MasterMainPageViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public ObservableCollection<MainPageMenuItem> MenuItems { get; set; }
        public ICommand GetSyncStatusCommand { get; private set; }

        public MasterMainPageViewModel()
        {
            GetSyncStatusCommand = new Command(getSyncStatusCommand);
            var collection = new PagesCollection();
            MenuItems = collection.GetPagesForMenu();
        }

        private void getSyncStatusCommand()
        {
            UpdateSyncStatus();
            DependencyService.Get<IToastService>().LongToast(SyncStatusText);
        }

        public void UpdateSyncStatus()
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SyncStatusText"));
        }
        public string SyncStatusText
        {
            get
            {
                string statusSyncKey = "SyncStatus";
                string status = "Not started";
                object objectStatus;
                if (Application.Current.Properties.TryGetValue(statusSyncKey, out objectStatus))
                {
                    status = (string) objectStatus;
                }
                return status;
            }
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartDialog()
        {
            MessagingCenter.Subscribe<SyncRouteStartMessage>(this, string.Empty, (msgSender) =>
                {
                    UpdateSyncStatus();
                });
            MessagingCenter.Subscribe<SyncRouteCompleteMessage>(this, string.Empty, (msgSender) =>
                {
                    UpdateSyncStatus();
                });
        }

        public void CloseDialog()
        {
            MessagingCenter.Unsubscribe<SyncRouteCompleteMessage>(this, string.Empty);
        }
        #endregion
    }
}
