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
        private List<string> _newRouteImgCollection = new List<string>();

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ShowNewRouteCommand { get; private set; }
        public ICommand GenerateNewRouteCommand { get; private set; }

        public bool IsBusy { get; set; }



        public MakeNewRouteAutoViewModel()
        {
            ShowNewRouteCommand = new Command(showNewRouteCommand);
            GenerateNewRouteCommand = new Command(generateNewRouteCommand);
            TokenStoreService tokenService = new TokenStoreService();
        }

        private void generateNewRouteCommand(object obj)
        {
            onTimerForUpdateImgNewRoute();
            //ImagesCacheDbManager imagesCache = new ImagesCacheDbManager(7);
            //imagesCache.Update();
        }

        private void showNewRouteCommand(object obj)
        {
            Navigation.PushModalAsync(new MakeNewRoutePage());
        }



        public void CloseDialog()
        {
         
        }

        public async void StartDialog()
        {
            _currentUserId = await _tokenService.GetUserIdAsync();
            //Хотел сделать при формирование маршрута при открытии страницы, но пока не получилось. Для отладки остановился на кнопке.
            /*if(NewRouteImgCollection.Count() == 0)
            {
                await refresh();
            }*/
        }

        private async Task refresh()
        {
            Device.StartTimer(TimeSpan.FromSeconds(5), onTimerForUpdateImgNewRoute);
        }

        private bool onTimerForUpdateImgNewRoute()
        {

            AutoRouteMakerManager routeMaker = new AutoRouteMakerManager();
            var autoRoute = routeMaker.Make(7, _currentUserId);
            NewRouteImgCollection.AddRange(autoRoute.Points.Select(p=>p.Images[0]));

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NewRouteImgCollection"));
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
    }

}
