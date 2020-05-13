using Acr.UserDialogs;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using QuestHelper.Consts;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Resources;
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

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ViewPhotoCommand { get; private set; }
        public ICommand CreateLast3Days { get; private set; }
        public ICommand CreateLast7Days { get; private set; }
        public ICommand CreateLast30Days { get; private set; }

        public bool IsBusy { get; set; }



        public MakeNewRouteAutoViewModel()
        {
            ViewPhotoCommand = new Command(viewPhotoAsync);
            CreateLast3Days = new Command(createLast3Days);
            CreateLast7Days = new Command(createLast7Days);
            CreateLast30Days = new Command(createLast30Days);
            TokenStoreService tokenService = new TokenStoreService();

        }

        private void createLast30Days(object obj)
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
        }

        public void CloseDialog()
        {
         
        }

        public async void StartDialog()
        {
            _currentUserId = await _tokenService.GetUserIdAsync();
        }

        public IEnumerable<NewRoutePoint> RoutePoints
        {
            get
            {
                //var list = _vpoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new MediaPreview() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType }).ToList();
                var list = new List<NewRoutePoint>();
                list.Add(new NewRoutePoint() { Name = "Точка 1"});
                /*list.Add(new NewRoutePoint() { Name = "Точка 2" });
                list.Add(new NewRoutePoint() { Name = "Точка 3" });
                list.Add(new NewRoutePoint() { Name = "Точка 4" });
                list.Add(new NewRoutePoint() { Name = "Точка 5" });
                list.Add(new NewRoutePoint() { Name = "Точка 6" });
                list.Add(new NewRoutePoint() { Name = "Точка 7" });
                list.Add(new NewRoutePoint() { Name = "Точка 8" });
                list.Add(new NewRoutePoint() { Name = "Точка 9" });*/
                return list;
            }
        }

        private void viewPhotoAsync(object imageSource)
        {

        }
    }

}
