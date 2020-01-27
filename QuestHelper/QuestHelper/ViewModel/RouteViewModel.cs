using Plugin.Geolocator;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.View;
using QuestHelper.WS;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using QuestHelper.Model.WS;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json.Linq;
using Acr.UserDialogs;
using System.IO;

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

        public INavigation Navigation { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;
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

        public RouteViewModel(string routeId, bool isFirstRoute)
        {
            _vroute = new ViewRoute(routeId);
            _isFirstRoute = isFirstRoute;
            ChooseImageForCoverCommand = new Command(chooseImageForCoverCommand);
            ShowNewRouteDialogCommand = new Command(showNewRouteData);
            AddNewRoutePointCommand = new Command(addNewRoutePointAsync);
            StartDialogCommand = new Command(startDialog);
            EditRouteCommand = new Command(editRouteCommandAsync);
            EditRouteCompleteCommand = new Command(editRouteCompleteCommand);
            CancelEditRouteCommand = new Command(cancelEditRouteCommand);
            ShareRouteCommand = new Command(shareRouteCommandAsync);
            FullScreenMapCommand = new Command(fullScreenMapCommandAsync);
            PhotoAlbumCommand = new Command(photoAlbumCommandAsync);
            BackNavigationCommand = new Command(backNavigationCommand);
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
                Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage() { RouteId = _vroute.Id, NeedCheckVersionRoute = true }, string.Empty);
            }
            IsVisibleModalRouteEdit = !IsVisibleModalRouteEdit;
            IsVisibleNavigationToolbar = !IsVisibleModalRouteEdit;
        }

        private void photoAlbumCommandAsync(object obj)
        {
            //var page = new RouteCarouselRootPage(_vroute.RouteId);
            //Navigation.PushModalAsync(page);
            var page = new RouteGalleryPage(_vroute.RouteId);
            Navigation.PushModalAsync(page);
        }

        internal async Task<bool> UserCanShareAsync()
        {
            TokenStoreService tokenService = new TokenStoreService();
            string userId = await tokenService.GetUserIdAsync();
            return userId.Equals(_vroute.CreatorId);
        }

        private async void shareRouteCommandAsync(object obj)
        {
            var shareRoutePage = new ShareRoutesServicesPage(_vroute.RouteId);
            await Navigation.PushModalAsync(shareRoutePage, true);
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

        public void startDialog()
        {
            if (!string.IsNullOrEmpty(_vroute.Name))
            {
                refreshRouteData();
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

        private void refreshRouteData()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            IsRefreshing = true;
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
            IsRefreshing = false;
        }
        void showNewRouteData()
        {
            SplashStartScreenIsVisible = false;
            RouteScreenIsVisible = !SplashStartScreenIsVisible;
            PointsOfRoute = new ObservableCollection<ViewRoutePoint>() { };
        }

        async void addNewRoutePointAsync()
        {
            //var routePointPage = new RoutePointPage(_vroute.Id, string.Empty);
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

        public bool IsVisibleNavigationToolbar
        {
            set
            {
                if (_isVisibleNavigationToolbar != value)
                {
                    _isVisibleNavigationToolbar = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsVisibleNavigationToolbar"));
                    }
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
                    //var page = new RoutePointPage(_vroute.Id, point.Id);
                    var page = new RoutePointV2Page(_vroute.Id, point.Id);
                    Navigation.PushModalAsync(page);
                    _selectedPoint = null;
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedRoutePointItem"));
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
                        PropertyChanged(this, new PropertyChangedEventArgs("ListIsRefreshing"));
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
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("NoPointWarningIsVisible"));
                    }
                }
            }
            get
            {
                return _noPointWarningIsVisible;
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
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("SplashStartScreenIsVisible"));
                    }
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
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("RouteScreenIsVisible"));
                    }
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
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("PointsOfRoute"));
                    }
                }
            }
            get
            {
                return _viewPointsOfRoute;
            }
        }
    }
}
