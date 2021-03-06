﻿using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.View;
using Syncfusion.ListView.XForms;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using static QuestHelper.Model.AutoGeneratedRouted;

namespace QuestHelper.ViewModel
{
    public class MakeNewRouteViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand BackNavigationCommand { get; private set; }
        public ICommand ViewPhotoCommand { get; private set; }
        public ICommand DeletePointCommand { get; private set; }
        public ICommand DeleteImageFromPointCommand { get; private set; }
        public ICommand ImagesTresholdReachedCommand { get; private set; }
        public ICommand SelectPointCommand { get; private set; }
        public ICommand SaveRouteCommand { get; private set; }

        TokenStoreService _tokenService = new TokenStoreService();
        private string _currentUserId;
        public bool IsBusy { get; set; }


        private AutoGeneratedRouted _autoGeneratedRoute;
        
        private AutoGeneratedRouted.AutoGeneratedPoint _selectedRoutePoint;

        public MakeNewRouteViewModel(AutoGeneratedRouted autoGeneratedRoute)
        {
            _autoGeneratedRoute = autoGeneratedRoute;
            BackNavigationCommand = new Command(backNavigationCommand);
            ViewPhotoCommand = new Command(viewPhotoAsync);
            DeletePointCommand = new Command(deletePointCommand);
            DeleteImageFromPointCommand = new Command(deleteImageFromPointCommand);
            SelectPointCommand = new Command(selectPointCommand);
            SaveRouteCommand = new Command(saveRouteCommand);
        }

        private void saveRouteCommand(object obj)
        {
            AutoRouteMakerManager maker = new AutoRouteMakerManager(new ImageManager());
            bool makeResult = maker.Make(_autoGeneratedRoute, _currentUserId);
            if (makeResult)
            {
                Navigation.PopModalAsync();
                Navigation.PushModalAsync(new RoutesPage());
            }
        }

        private void selectPointCommand(object obj)
        {
            var item = (ItemSelectionChangedEventArgs)obj;
            SelectedRoutePoint = (AutoGeneratedPoint)item.AddedItems.FirstOrDefault();
        }

        private void deletePointCommand(object obj)
        {
            var point = (AutoGeneratedPoint)obj;
            point.IsDeleted = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RoutePoints"));
        }
        
        private void deleteImageFromPointCommand(object obj)
        {
            var currentImage = (AutoGeneratedImage)obj;
            currentImage.IsDeleted = !currentImage.IsDeleted;
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }


        public void CloseDialog()
        {
         
        }

        public async void StartDialog()
        {
            _currentUserId = await _tokenService.GetUserIdAsync();
        }

        public ObservableCollection<AutoGeneratedRouted.AutoGeneratedPoint> RoutePoints => _autoGeneratedRoute.Points;

        private void viewPhotoAsync(object imageSource)
        {
            string path = string.Empty;
            var defaultViewerService = DependencyService.Get<IDefaultViewer>();
            if (imageSource is GalleryImage)
            {
                path = ((GalleryImage)imageSource).ImagePath;
            }
            if (!string.IsNullOrEmpty(path))
            {
                defaultViewerService.Show(path.Replace("_preview", ""));
            }
        }

        public AutoGeneratedRouted.AutoGeneratedPoint SelectedRoutePoint
        {
            set
            {
                if (value != _selectedRoutePoint)
                {
                    _selectedRoutePoint = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRoutePoint"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRoutePointImages"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GridItemsPreviewCount"));
                }
            }
            get => _selectedRoutePoint;
        }

        public ObservableCollection<AutoGeneratedImage> SelectedRoutePointImages
        {
            get
            {
                return _selectedRoutePoint != null ? _selectedRoutePoint.Images : new ObservableCollection<AutoGeneratedImage>();
            }
        }

        public string Name
        {
            get
            {
                return _autoGeneratedRoute.Name;
            }
        }

        public int GridItemsPreviewCount
        {
            get
            {
                int maxComfortableCount = 4;
                int count = maxComfortableCount;
                int countImages = SelectedRoutePointImages.Count;
                if ((countImages > 0) && (countImages < count)) count = countImages;
                return count;
                //return 2;
            }
        }
    }
}
