using QuestHelper.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace QuestHelper.View
{
    public partial class MasterMainPage : ContentPage
    {
        public ListView ListView;

        public MasterMainPage()
        {
            InitializeComponent();
            BindingContext = new MasterMainPageViewModel();
            ListView = MenuItemsListView;
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }

    class MasterMainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MainPageMenuItem> MenuItems { get; set; }

        public MasterMainPageViewModel()
        {
            var collection = new PagesCollection();
            MenuItems = collection.GetPagesForMenu();
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
