using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.LocalDB.Model;
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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuestHelper.Model;
using Microsoft.AppCenter.Analytics;
using QuestHelper.Model.Messages;
using System.IO;
using Xamarin.Essentials;
using Acr.UserDialogs;
using QuestHelper.Resources;

namespace QuestHelper.ViewModel
{
    public class EditPoiViewModel : INotifyPropertyChanged, IDialogEvents
    {
        ViewRoutePoint _vpoint;
        ViewPoi _vpoi;
        bool _isNewPoi;
        string _authToken = string.Empty;
        string _userId = string.Empty;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand UpdatePoiCommand { get; private set; }

        public EditPoiViewModel(string poiId, string routePointId)
        {
            BackNavigationCommand = new Command(backNavigationCommand);
            DeleteCommand = new Command(deleteCommand);
            UpdatePoiCommand = new Command(updatePoiCommand);

            _vpoi = new ViewPoi(poiId);

            _isNewPoi = string.IsNullOrEmpty(_vpoi.Name);

            if (!string.IsNullOrEmpty(routePointId))
            {
                _vpoi.ByRoutePointId = routePointId;
                _vpoint = new ViewRoutePoint();
                _vpoint.Refresh(_vpoi.ByRoutePointId);
            }
        }

        private async void updatePoiCommand(object obj)
        {
            PoiApiRequest poiApi = new PoiApiRequest(_authToken);
            _vpoi.UpdateDate = DateTimeOffset.Now;
            bool resultUpload = await poiApi.UploadPoiAsync(_vpoi.GetJsonStructure());
            if (resultUpload)
            {
                applyChanges();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UserDialogs.Instance.Alert(CommonResource.PoiMsg_Warning, CommonResource.PoiMsg_Saved, CommonResource.CommonMsg_Ok);
                });

                MessagingCenter.Send<PoiUpdatedMessage>(new PoiUpdatedMessage() { PoiId = _vpoi.Id }, string.Empty);
                await Navigation.PopModalAsync();
            }
        }

        private async void deleteCommand(object obj)
        {
            PoiApiRequest poiApi = new PoiApiRequest(_authToken);
            bool resultDelete = await poiApi.DeleteAsync(_vpoi.Id);
            if (resultDelete)
            {
                deletePoi();
                MessagingCenter.Send<PoiUpdatedMessage>(new PoiUpdatedMessage() { PoiId = _vpoi.Id }, string.Empty);
            }
            await Navigation.PopModalAsync();
        }

        private void deletePoi()
        {
            PoiManager poiManager = new PoiManager();
            poiManager.Delete(_vpoi.Id);
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }

        public string PoiName
        {
            set
            {
                if (_vpoi.Name != value)
                {
                    _vpoi.Name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PoiName"));
                }
            }
            get
            {
                return _vpoi.Name;
            }
        }

        public string PoiDescription
        {
            set
            {
                if (_vpoi.Description != value)
                {
                    _vpoi.Description = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PoiDescription"));
                }
            }
            get
            {
                return _vpoi.Description;
            }
        }

        public string PoiImage
        {
            get
            {
                return Path.Combine(ImagePathManager.GetPicturesDirectory(), _vpoi.ImgFilename);
            }
            set
            {
                if (!value.Equals(_vpoi.ImgFilename))
                {
                    _vpoi.ImgFilename = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PoiImage"));
                }
            }
        }


        private void applyChanges()
        {
            Analytics.TrackEvent("Poi updated", new Dictionary<string, string> { { "Name", "Id" }, { _vpoi.Name, _vpoi.Id} });
            _vpoi.UpdateDate = DateTimeOffset.Now;
            _vpoi.Save();
        }

        public async void StartDialog()
        {
            TokenStoreService tokenService = new TokenStoreService();
            _authToken = await tokenService.GetAuthTokenAsync();
            _userId = await tokenService.GetUserIdAsync();
            if (_isNewPoi)
            {
                PoiName = _vpoint.Name;
                PoiDescription = _vpoint.Description;                
                PoiImage = ImagePathManager.GetMediaFilename(_vpoint.ImageMediaId, _vpoint.ImageMediaType, true);
                _vpoi.Location = new Xamarin.Forms.Maps.Position(_vpoint.Latitude, _vpoint.Longitude);
                _vpoi.CreatorId = _userId;
                string pathToImg = Path.Combine(ImagePathManager.GetPicturesDirectory(), _vpoi.ImgFilename);
                if (File.Exists(pathToImg))
                {
                    var bytes = File.ReadAllBytes(pathToImg);
                    _vpoi.ImgBase64 = Convert.ToBase64String(bytes);
                }
            }
        }

        public void CloseDialog()
        {
        }

        public bool IsPublished
        {
            set
            {
                if (_vpoi.IsPublished != value)
                {
                    _vpoi.IsPublished = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsPublished"));
                }
            }
            get
            {
                return _vpoi.IsPublished;
            }
        }

    }
}
