using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace QuestHelper
{
    public partial class MasterMainPage : ContentPage
    {
        public ListView ListView;

        public MasterMainPage()
        {
            InitializeComponent();
            BindingContext = new MasterMainPageViewModel();
            ListView = MenuItemsListView;
        }
    }

    class MasterMainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<MainPageMenuItem> MenuItems { get; set; }

        public MasterMainPageViewModel()
        {
            MenuItems = new ObservableCollection<MainPageMenuItem>(new[]
            {
                    
                new MainPageMenuItem { Id = 1, Title = "меню 1", TargetType = typeof(MapOverviewPage)},

                new MainPageMenuItem { Id = 0, Title = "меню 2", TargetType = typeof(QuestHelperPage)}
                    
                });
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
