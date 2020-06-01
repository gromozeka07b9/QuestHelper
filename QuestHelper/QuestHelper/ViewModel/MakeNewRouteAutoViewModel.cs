using Acr.UserDialogs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using QuestHelper.Consts;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Resources;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class MakeNewRouteAutoViewModel : INotifyPropertyChanged, IDialogEvents
    {
        TokenStoreService _tokenService = new TokenStoreService();
        private string _currentUserId;
        private List<string> _randomImgCollection = new List<string>();
        private List<string> _newRouteImgCollection = new List<string>();
        private bool _viewModelBackgroundStarted;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ViewPhotoCommand { get; private set; }
        //public ICommand CreateLast3Days { get; private set; }
        //public ICommand CreateLast7Days { get; private set; }
        //public ICommand CreateLast30Days { get; private set; }
        public ICommand ShowGalleryForMakeAlbumCommand { get; private set; }
        public ICommand ShowAutoAlbumCommand { get; private set; }

        public bool IsBusy { get; set; }



        public MakeNewRouteAutoViewModel()
        {
            ViewPhotoCommand = new Command(viewPhotoAsync);
            //CreateLast3Days = new Command(createLast3Days);
            //CreateLast7Days = new Command(createLast7Days);
            //CreateLast30Days = new Command(createLast30Days);
            ShowGalleryForMakeAlbumCommand = new Command(showGalleryForMakeAlbumCommand);
            ShowAutoAlbumCommand = new Command(showAutoAlbumCommand);
            TokenStoreService tokenService = new TokenStoreService();
            //_viewModelBackgroundStarted = true;
        }

        private void showGalleryForMakeAlbumCommand(object obj)
        {
            Navigation.PushModalAsync(new MakeNewRoutePage());
        }

        private void showAutoAlbumCommand(object obj)
        {
        }

        /*private void createLast30Days(object obj)
        {
            AutoRouteMakerManager routeMaker = new AutoRouteMakerManager();
            bool result = routeMaker.Make(30, _currentUserId);
            if (result)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.Alert("Внимание", "Маршрут с фотографиями за 30 дней создан", CommonResource.CommonMsg_Ok);
                });
            }
        }

        private void createLast7Days(object obj)
        {
            AutoRouteMakerManager routeMaker = new AutoRouteMakerManager();
            bool result = routeMaker.Make(7, _currentUserId);
            if (result)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.Alert("Внимание", "Маршрут с фотографиями за 7 дней создан", CommonResource.CommonMsg_Ok);
                });
            }
        }

        private void createLast3Days(object obj)
        {
            AutoRouteMakerManager routeMaker = new AutoRouteMakerManager();
            bool result = routeMaker.Make(3, _currentUserId);
            if (result)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.Alert("Внимание", "Маршрут с фотографиями за 3 дней создан", CommonResource.CommonMsg_Ok);
                });
            }
        }*/

        public void CloseDialog()
        {
         
        }

        public async void StartDialog()
        {
            _currentUserId = await _tokenService.GetUserIdAsync();
            if(NewRouteImgCollection.Count() == 0)
            {
                await refresh();
            }
        }

        private void viewPhotoAsync(object imageSource)
        {

        }

        private async Task refresh()
        {
            Device.StartTimer(TimeSpan.FromSeconds(5), onTimerForUpdateImgNewRoute);
            //Device.StartTimer(TimeSpan.FromMilliseconds(50000), onTimerForUpdateImgRandom);
        }

        private bool onTimerForUpdateImgNewRoute()
        {
            ImagesDataStoreManager imagesGalleryManager = new ImagesDataStoreManager(7, false, 7);
            imagesGalleryManager.LoadListImages();
            NewRouteImgCollection = imagesGalleryManager.GetItems(0).Select(x => x.ImagePath).ToList();

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLoadingNewRouteData"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountImagesInNewRouteText"));
            return false;
        }
        private bool onTimerForUpdateImgRandom()
        {
            ImagesDataStoreManager imagesGalleryManager = new ImagesDataStoreManager(10, false, 200);
            imagesGalleryManager.LoadListImages();
            var items = imagesGalleryManager.GetRandomItems(10);
            RandomImgCollection = items.Select(x => x.ImagePath).ToList();
            return false;
        }

        public bool IsLoadingNewRouteData
        {
            get
            {
                return NewRouteImgCollection.Count == 0;
            }
        }

        public string CountImagesInNewRouteText
        {
            get
            {
                return $"Ваш маршрут за неделю состоит из {NewRouteImgCollection.Count.ToString()} фотографий. Хотите просмотреть его?";
            }
        }
        public List<string> NewRouteImgCollection
        {
            set
            {
                if(value != _newRouteImgCollection)
                {
                    _newRouteImgCollection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewRouteImgCollection"));
                }
            }
            get
            {
                return _newRouteImgCollection;
            }

        }
        public List<string> RandomImgCollection
        {
            set
            {
                if(value != _randomImgCollection)
                {
                    _randomImgCollection = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RandomImgCollection"));
                }
            }
            get
            {
                return _randomImgCollection;
            }

        }
    }

}
