using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.View;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Acr.UserDialogs;
using System.IO;
using QuestHelper.Managers.Sync;
using QuestHelper.Resources;
using Xamarin.Essentials;

namespace QuestHelper.ViewModel
{
    public class RouteViewModel : INotifyPropertyChanged
    {
        private bool _splashStartScreenIsVisible;
        private bool _routeScreenIsVisible;
        private ObservableCollection<ViewRoutePoint> _viewPointsOfRoute = new ObservableCollection<ViewRoutePoint>();

        private bool _isFirstRoute;

        private ViewRoute _vroute;
        private ViewRoutePoint _selectedPoint;
        private RouteManager _routeManager = new RouteManager();
        private RoutePointManager _routePointManager = new RoutePointManager();
        private RoutePointMediaObjectManager _routePointMediaObjectManager = new RoutePointMediaObjectManager();
        private bool _listIsRefreshing;
        private bool _noPointWarningIsVisible;
        private bool _isRefreshing;
        private bool _isVisibleModalRouteEdit = false;
        private bool _isVisibleNavigationToolbar = true;
        private string _descriptionForEdit;
        private string _nameForEdit;
        private string _coverImagePathForEdit;
        private string _imgFilenameForEdit;
        private bool _isNeedSyncRoute = false;
        private bool _isNeedSyncRouteInitial = false;

        public INavigation Navigation { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand SyncRouteCommand { get; private set; }
        public ICommand ChooseImageForCoverCommand { get; private set; }
        public ICommand ShowNewRouteDialogCommand { get; private set; }
        public ICommand AddNewRoutePointCommand { get; private set; }
        public ICommand StartDialogCommand { get; private set; }

        public ICommand EditRouteCommand { get; private set; }
        public ICommand EditRouteCompleteCommand { get; private set; }
        public ICommand CancelEditRouteCommand { get; private set; }
        public ICommand ShareRouteCommand { get; private set; }
        public ICommand FullScreenMapCommand { get; private set; }
        public ICommand PhotoAlbumCommand { get; private set; }
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand ShareCommand { get; private set; }
        
        public ICommand DeleteRouteCommand { get; private set; }
        public RouteViewModel(string routeId, bool isFirstRoute, bool isNeedSyncRoute)
        {
            _vroute = new ViewRoute(routeId);
            _isFirstRoute = isFirstRoute;
            _vroute.ServerSynced = !isNeedSyncRoute;
            _isNeedSyncRouteInitial = isNeedSyncRoute;
            _isNeedSyncRoute = _isNeedSyncRouteInitial;
            //ShareCommand = new Command(shareCommandAsync);
            ShareRouteCommand = new Command(shareRouteCommandAsync);
            SyncRouteCommand = new Command(syncRouteCommandAsync);
            ChooseImageForCoverCommand = new Command(chooseImageForCoverCommand);
            ShowNewRouteDialogCommand = new Command(showNewRouteData);
            AddNewRoutePointCommand = new Command(addNewRoutePointAsync);
            StartDialogCommand = new Command(startDialogAsync);
            EditRouteCommand = new Command(editRouteCommandAsync);
            EditRouteCompleteCommand = new Command(editRouteCompleteCommand);
            CancelEditRouteCommand = new Command(cancelEditRouteCommand);
            FullScreenMapCommand = new Command(fullScreenMapCommandAsync);
            PhotoAlbumCommand = new Command(photoAlbumCommandAsync);
            BackNavigationCommand = new Command(backNavigationCommand);
            DeleteRouteCommand = new Command(deleteRouteCommand);
        }

        private async void shareRouteCommandAsync(object obj)
        {
            if ((IsNeedSyncRoute) && (!IsRefreshing))
            {
                await syncRouteAsync(_vroute.Id).ContinueWith(async (result) =>
                {
                    if (result.IsCompleted)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            await startShareRouteDialogAsync(_vroute.RouteId);
                        });
                    }
                });
            }
            else
            {
                await startShareRouteDialogAsync(_vroute.RouteId);
            }
        }

        private async Task startShareRouteDialogAsync(string routeId)
        {
            var shareRoutePage = new ShareRoutesServicesPage(routeId);
            await Navigation.PushModalAsync(shareRoutePage, true);
        }

        private async void syncRouteCommandAsync(object obj)
        {
            await syncRouteAsync(_vroute.Id);
        }
        
        private async void deleteRouteCommand(object obj)
        {
            if (!string.IsNullOrEmpty(_vroute.ObjVerHash))
            {
                UserDialogs.Instance.Alert(new AlertConfig(){ Message = "Маршрут уже синхронизирован и не может быть удален", Title = "Внимание!"});
            }
            else
            {
                bool deleteRoute = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Message = "Вы уверены что хотите удалить маршрут?", Title = "Внимание!", OkText = CommonResource.CommonMsg_Yes, CancelText = CommonResource.CommonMsg_No });
                if (deleteRoute)
                {
                    _routeManager.DeleteRoutesDataFromStorage(new ViewRoute[]{_vroute});
                    backNavigationCommand(new object());
                    UserDialogs.Instance.Alert(new AlertConfig() {Message = "Маршрут успешно удален", Title = "Внимание!"});                    
                }
            }
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }

        private async void chooseImageForCoverCommand(object obj)
        {
            ImageManager imageManager = new ImageManager();
            var pickPhoto = await imageManager.PickPhotoAsync();
            if (pickPhoto.pickPhotoResult)
            {
                ImgFilenameForEdit = ImagePathManager.GetMediaFilename(pickPhoto.newMediaId, MediaObjectTypeEnum.Image, false);
                CoverImagePathForEdit = Path.Combine(ImagePathManager.GetPicturesDirectory(), ImgFilenameForEdit);
            }
        }

        private void cancelEditRouteCommand(object obj)
        {
            IsVisibleModalRouteEdit = !IsVisibleModalRouteEdit;
            IsVisibleNavigationToolbar = !IsVisibleModalRouteEdit;
        }

        private void editRouteCompleteCommand(object obj)
        {
            if (!NameForEdit.Equals(Name) || 
                (DescriptionForEdit !=null && !DescriptionForEdit.Equals(Description)) ||
                (!string.IsNullOrEmpty(ImgFilename) && string.IsNullOrEmpty(ImgFilenameForEdit)) ||
                (!string.IsNullOrEmpty(ImgFilenameForEdit) && string.IsNullOrEmpty(ImgFilename)) ||
                (ImgFilenameForEdit != null && ImgFilename !=null && !ImgFilenameForEdit.Equals(ImgFilename)))
            {
                Description = DescriptionForEdit;
                _vroute.Name = NameForEdit;
                _vroute.Description = DescriptionForEdit;
                _vroute.ImgFilename = ImgFilenameForEdit;
                _vroute.Version++;
                _vroute.ObjVerHash = string.Empty;
                _vroute.Save();
                IsNeedSyncRoute = true;
                //Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage() { }, string.Empty);
            }
            IsVisibleModalRouteEdit = !IsVisibleModalRouteEdit;
            IsVisibleNavigationToolbar = !IsVisibleModalRouteEdit;
        }

        private void photoAlbumCommandAsync(object obj)
        {
            var page = new RouteCarouselRootPage(_vroute.RouteId);
            Navigation.PushModalAsync(page);
        }

        internal async Task<bool> UserCanShareAsync()
        {
            TokenStoreService tokenService = new TokenStoreService();
            string userId = await tokenService.GetUserIdAsync();
            return userId.Equals(_vroute.CreatorId);
        }
        
        private void fullScreenMapCommandAsync(object obj)
        {
            var mapRoutePage = new MapRouteOverviewPage(_vroute.RouteId);
            Navigation.PushModalAsync(mapRoutePage, true);
        }

        private void editRouteCommandAsync(object obj)
        {
            IsVisibleModalRouteEdit = !IsVisibleModalRouteEdit;
            IsVisibleNavigationToolbar = !IsVisibleModalRouteEdit;
            if (IsVisibleModalRouteEdit)
            {
                NameForEdit = Name;
                DescriptionForEdit = Description;
                CoverImagePathForEdit = CoverImage;
                ImgFilenameForEdit = ImgFilename;
            }
        }

        public async void startDialogAsync()
        {
            _vroute.Refresh(_vroute.Id);
            if (_isNeedSyncRouteInitial)
            {
                IsNeedSyncRoute = _isNeedSyncRouteInitial;
                _isNeedSyncRouteInitial = false;
            }
            else
            {
                IsNeedSyncRoute = string.IsNullOrEmpty(_vroute.ObjVerHash);
            }
            if (!string.IsNullOrEmpty(_vroute.Name))
            {
                await refreshRouteDataAsync();
            }
            else
            {
                _vroute.Name = "";
                if (_isFirstRoute)
                    showNewRouteWarningDialog();
                else showNewRouteData();
            }
            ListIsRefreshing = false;
        }
        internal void closeDialog()
        {
        }

        private void showNewRouteWarningDialog()
        {
            SplashStartScreenIsVisible = true;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
        }

        private async Task refreshRouteDataAsync()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            //IsNeedSyncRoute = false;
            IsRefreshing = true;
            updatePoints();
            IsRefreshing = false;
        }
        private async Task syncRouteAsync(string routeId)
        {
            IsNeedSyncRoute = false;
            IsRefreshing = true;
            SyncServer syncSrv = new SyncServer();
            await syncSrv.Sync(_vroute.Id, false).ContinueWith(result =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (!result.Result)
                    {
                        IsNeedSyncRoute = true;
                        UserDialogs.Instance.Alert("Ошибка синхронизации", "Внимание", "Ok");
                    }
                    else
                    {
                        updatePoints();
                    }
                    IsRefreshing = false;
                });

            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        private void updatePoints()
        {
            var points = _routePointManager.GetPointsByRouteId(_vroute.Id);
            if (!points.Any())
            {
                PointsOfRoute = new ObservableCollection<ViewRoutePoint>();
            }
            else
            {
                PointsOfRoute = new ObservableCollection<ViewRoutePoint>(points);
            }

            NoPointWarningIsVisible = PointsOfRoute.Count == 0;
        }

        void showNewRouteData()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            PointsOfRoute = new ObservableCollection<ViewRoutePoint>() { };
        }

        async void addNewRoutePointAsync()
        {
            var routePointPage = new RoutePointV2Page(_vroute.Id, string.Empty);
            await Navigation.PushModalAsync(routePointPage, true);
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

        public bool IsVisibleNavigationToolbar
        {
            set
            {
                if (_isVisibleNavigationToolbar != value)
                {
                    _isVisibleNavigationToolbar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisibleNavigationToolbar"));
                }
            }
            get
            {
                return _isVisibleNavigationToolbar;
            }
        }
        public string Name
        {
            set
            {
                if (_vroute.Name != value)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _vroute.Name;
            }
        }
        public string NameForEdit
        {
            set
            {
                if (_nameForEdit != value)
                {
                    _nameForEdit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NameForEdit"));
                }
            }
            get
            {
                return _nameForEdit;
            }
        }
        public string DescriptionForEdit
        {
            set
            {
                if (_descriptionForEdit != value)
                {
                    _descriptionForEdit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DescriptionForEdit"));
                }
            }
            get
            {
                return _descriptionForEdit;
            }
        }
        public string Description
        {
            set
            {
                if (_vroute.Description != value)
                {
                    _vroute.Description = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                }
            }
            get
            {
                return _vroute.Description;
            }
        }

        public string CoverImagePathForEdit
        {
            set
            {
                if (_coverImagePathForEdit != value)
                {
                    _coverImagePathForEdit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CoverImagePathForEdit"));
                }
            }
            get
            {
                return _coverImagePathForEdit;
            }
        }

        public string CoverImage
        {
            get
            {
                return _vroute.CoverImage;
            }
        }
        
        public string ImgFilename
        {
            set
            {
                if (_vroute.ImgFilename != value)
                {
                    _vroute.ImgFilename = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImgFilename"));
                }
            }
            get
            {
                return _vroute.ImgFilename;
            }
        }
        
        public string ImgFilenameForEdit
        {
            set
            {
                if (_imgFilenameForEdit != value)
                {
                    _imgFilenameForEdit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImgFilenameForEdit"));
                }
            }
            get
            {
                return _imgFilenameForEdit;
            }
        }

        /*public string RouteLength
      {
          set
          {
              if (_routeLength != value)
              {
                  _routeLength = value;
                  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RouteLength"));
              }
          }
          get
          {
              return _routeLength;
          }
      }
      public string RouteLengthSteps
      {
          set
          {
              if (_routeLengthSteps != value)
              {
                  _routeLengthSteps = value;
                  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RouteLengthSteps"));
              }
          }
          get
          {
              return _routeLengthSteps;
          }
      }
      public int CountOfPhotos
      {
          set
          {
              if (_countOfPhotos != value)
              {
                  _countOfPhotos = value;
                  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPhotos"));
              }
          }
          get
          {
              return _countOfPhotos;
          }
      }
      public int CountOfPoints
      {
          set
          {
              if (_countOfPoints != value)
              {
                  _countOfPoints = value;
                  PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountOfPoints"));
              }
          }
          get
          {
              return _countOfPoints;
          }
      }*/

        public ViewRoutePoint SelectedRoutePointItem
        {
            set
            {
                if(_selectedPoint != value)
                {
                    ViewRoutePoint point = value;
                    var page = new RoutePointV2Page(_vroute.Id, point.Id);
                    Navigation.PushModalAsync(page);
                    _selectedPoint = null;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRoutePointItem"));
                }
            }
        }

        public bool ListIsRefreshing
        {
            set
            {
                if (_listIsRefreshing != value)
                {
                    _listIsRefreshing = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ListIsRefreshing"));
                    }
                }
            }
            get
            {
                return _listIsRefreshing;
            }
        }
        public bool NoPointWarningIsVisible
        {
            set
            {
                if (_noPointWarningIsVisible != value)
                {
                    _noPointWarningIsVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NoPointWarningIsVisible"));
                }
            }
            get
            {
                return _noPointWarningIsVisible;
            }
        }

        public bool IsNeedSyncRoute
        {
            set
            {
                if (_isNeedSyncRoute != value)
                {
                    _isNeedSyncRoute = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsNeedSyncRoute"));
                }
            }
            get
            {
                return _isNeedSyncRoute;
            }
        }
        public bool IsVisibleModalRouteEdit
        {
            set
            {
                if (_isVisibleModalRouteEdit != value)
                {
                    _isVisibleModalRouteEdit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisibleModalRouteEdit"));
                }
            }
            get
            {
                return _isVisibleModalRouteEdit;
            }
        }

        public bool SplashStartScreenIsVisible
        {
            set
            {
                if (_splashStartScreenIsVisible != value)
                {
                    _splashStartScreenIsVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SplashStartScreenIsVisible"));
                }
            }
            get
            {
                return _splashStartScreenIsVisible;
            }
        }
        public bool RouteScreenIsVisible
        {
            set
            {
                if (_routeScreenIsVisible != value)
                {
                    _routeScreenIsVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RouteScreenIsVisible"));
                }
            }
            get
            {
                return _routeScreenIsVisible;
            }
        }
        public ObservableCollection<ViewRoutePoint> PointsOfRoute
        {
            set
            {
                if (_viewPointsOfRoute != value)
                {
                    _viewPointsOfRoute = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PointsOfRoute"));
                }
            }
            get
            {
                return _viewPointsOfRoute;
            }
        }
    }
}
