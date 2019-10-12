using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    internal class AlbumsViewModel: INotifyPropertyChanged
    {
        private IEnumerable<ViewRoute> _routes;
        private ViewRoute _routeItem;
        private RouteManager _routeManager = new RouteManager();
        private bool _noPostsWarningIsVisible;
        private bool _isRefreshing;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand RefreshListPostsCommand { get; private set; }

        internal AlbumsViewModel()
        {
            RefreshListPostsCommand = new Command(refreshListPostsCommand);

        }

        private void refreshListPostsCommand()
        {
            IsRefreshing = true;
            Routes = _routeManager.GetPosts();
            NoPostsWarningIsVisible = Routes.Count() == 0;
            IsRefreshing = false;
        }

        internal void startDialog()
        {
            refreshListPostsCommand();
        }
        internal void closeDialog()
        {
        }
        public bool IsRefreshing
        {
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsRefreshing"));
                    }
                }
            }
            get
            {
                return _isRefreshing;
            }
        }
        public bool NoPostsWarningIsVisible
        {
            set
            {
                if (_noPostsWarningIsVisible != value)
                {
                    _noPostsWarningIsVisible = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("NoPostsWarningIsVisible"));
                    }
                }
            }
            get
            {
                return _noPostsWarningIsVisible;
            }
        }
        public ViewRoute SelectedRouteItem
        {
            set
            {
                if (_routeItem != value)
                {
                    _routeItem = value;
                    var coverPage = new RouteCoverPage(_routeItem.RouteId);
                    Navigation.PushAsync(coverPage);
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedRouteItem"));
                    _routeItem = null;
                }
            }
        }
        public IEnumerable<ViewRoute> Routes
        {
            set
            {
                if (_routes != value)
                {
                    _routes = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Routes"));
                        NoPostsWarningIsVisible = _routes.Count() > 0;
                    }
                }
            }
            get
            {
                return _routes;
            }
        }

    }
}
