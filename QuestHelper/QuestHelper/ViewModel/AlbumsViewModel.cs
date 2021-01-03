using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.Resources;
using QuestHelper.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    /// <summary>
    /// Альбомы - список уже загруженных публикаций
    /// </summary>
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
        public ICommand DeleteDataAllAlbumsCommand { get; private set; }

        internal AlbumsViewModel()
        {
            RefreshListPostsCommand = new Command(refreshListPostsCommandAsync);
            DeleteDataAllAlbumsCommand = new Command(deleteDataAllAlbumsCommandAsync);
        }

        private async void deleteDataAllAlbumsCommandAsync()
        {
            bool answerYesIsNo = await Application.Current.MainPage.DisplayAlert(CommonResource.CommonMsg_Warning, CommonResource.Albums_AreYourSureToDeleteLoadedRoutes, CommonResource.CommonMsg_No, CommonResource.CommonMsg_Yes);
            if (!answerYesIsNo) //порядок кнопок - хардкод, и непонятно, почему именно такой
            {
                TokenStoreService token = new TokenStoreService();
                string userId = await token.GetUserIdAsync();
                var routesForDelete = _routeManager.GetPostsOtherCreators(userId);
                _routeManager.DeleteRoutesDataFromStorage(routesForDelete);
                refreshListPostsCommandAsync();
            }
        }

        private async void refreshListPostsCommandAsync()
        {
            IsRefreshing = true;
            TokenStoreService token = new TokenStoreService();
            string userId = await token.GetUserIdAsync();
            Routes = _routeManager.GetPostsOtherCreators(userId);
            NoPostsWarningIsVisible = Routes.Count() == 0;
            IsRefreshing = false;
        }

        internal void startDialog()
        {
            refreshListPostsCommandAsync();
            MessagingCenter.Subscribe<SyncRouteCompleteMessage>(this, string.Empty, (sender) =>
            {
                if (string.IsNullOrEmpty(sender.RouteId) && sender.SuccessSync)
                {
                    refreshListPostsCommandAsync();
                }
            });
        }
        internal void closeDialog()
        {
            MessagingCenter.Unsubscribe<SyncRouteCompleteMessage>(this, string.Empty);
        }
        public bool IsRefreshing
        {
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRefreshing"));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoPostsWarningIsVisible"));
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
                    var coverPage = new RouteCoverPage(value);
                    Navigation.PushModalAsync(coverPage);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRouteItem"));
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
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Routes"));
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
