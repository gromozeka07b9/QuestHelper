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
                new MainPageMenuItem { Id = 0, Title = "Карта", TargetType = typeof(MapOverviewPage)},
                new MainPageMenuItem { Id = 1, Title = "Маршруты", TargetType = typeof(MapOverviewPage)},
                new MainPageMenuItem { Id = 2, Title = "Новый маршрут", TargetType = typeof(MapOverviewPage)},
                new MainPageMenuItem { Id = 3, Title = "Что интересного вокруг", TargetType = typeof(MapOverviewPage)},
                new MainPageMenuItem { Id = 4, Title = "Профиль", TargetType = typeof(MapOverviewPage)},
                new MainPageMenuItem { Id = 5, Title = "О нас", TargetType = typeof(MapOverviewPage)},
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
