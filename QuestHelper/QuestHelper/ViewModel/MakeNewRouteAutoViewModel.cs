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

        public ICommand ShowNewRouteCommand { get; private set; }

        public bool IsBusy { get; set; }



        public MakeNewRouteAutoViewModel()
        {
            ShowNewRouteCommand = new Command(showNewRouteCommand);
            TokenStoreService tokenService = new TokenStoreService();
            //_viewModelBackgroundStarted = true;
        }

        private void showNewRouteCommand(object obj)
        {
            Navigation.PushModalAsync(new MakeNewRoutePage());
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
            /*ImagesDataStoreManager imagesGalleryManager = new ImagesDataStoreManager(7, false, 7);
            imagesGalleryManager.LoadListImages();
            NewRouteImgCollection = imagesGalleryManager.GetItems(0).Select(x => x.ImagePath).ToList();*/

            AutoRouteMakerManager routeMaker = new AutoRouteMakerManager();
            var autoRoute = routeMaker.Make(7, _currentUserId);
            NewRouteImgCollection.AddRange(autoRoute.Points.Select(p=>p.Images[0]));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsLoadingNewRouteData"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountImagesInNewRouteText"));
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
        /*public List<string> RandomImgCollection
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

        }*/
    }

}
