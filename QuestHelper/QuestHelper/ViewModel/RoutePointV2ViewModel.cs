using Acr.UserDialogs;
using Microsoft.AppCenter.Analytics;
using Plugin.Media;
using Plugin.Media.Abstractions;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.Resources;
using QuestHelper.View;
using QuestHelper.View.Geo;
using System;
using System.Collections.Generic;
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
    public class RoutePointV2ViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; internal set; }
        public ICommand DeleteCommand { get; internal set; }
        public ICommand TakePhotoCommand { get; private set; }
        public ICommand ViewPhotoCommand { get; private set; }
        public ICommand PlayMediaCommand { get; private set; }
        public ICommand DeletePhotoCommand { get; private set; }
        public ICommand DeletePointCommand { get; private set; }
        public ICommand EditNameCommand { get; private set; }
        public ICommand CancelNameCommand { get; private set; }
        public ICommand ClearNameCommand { get; private set; }
        public ICommand EditNameCompleteCommand { get; private set; }
        public ICommand EditDescriptionCommand { get; private set; }
        public ICommand CopyCoordinatesCommand { get; private set; }
        public ICommand UpdateAddressCommand { get; private set; }
        public ICommand CopyAddressCommand { get; private set; }
        public ICommand AddPhotoCommand { get; private set; }
        public ICommand AddAudioCommand { get; private set; }
        public ICommand ShareCommand { get; private set; }

        GeolocatorManager _geolocatorManager = new GeolocatorManager();
        ViewRoutePoint _vpoint;
        private bool _isVisibleModalNameEdit;
        private bool _newPoint;
        private string _nameForEdit;

        public RoutePointV2ViewModel(string routeId, string routePointId)
        {
            BackNavigationCommand = new Command(backNavigationCommand);
            TakePhotoCommand = new Command(takePhotoAsync);
            ViewPhotoCommand = new Command(viewPhotoAsync);
            PlayMediaCommand = new Command(playMediaAsync);
            DeletePhotoCommand = new Command(deletePhotoAsync);
            DeletePointCommand = new Command(deletePoint);
            AddPhotoCommand = new Command(addPhotoAsync);
            AddAudioCommand = new Command(addAudioAsync);
            ShareCommand = new Command(shareCommand);
            EditNameCommand = new Command(editNameCommand);
            CancelNameCommand = new Command(cancelNameCommand);
            EditNameCompleteCommand = new Command(editNameCompleteCommand);
            ClearNameCommand = new Command(clearNameCommand);
            EditDescriptionCommand = new Command(editDescriptionCommand);
            CopyCoordinatesCommand = new Command(copyCoordinatesCommand);
            UpdateAddressCommand = new Command(updateAddressCommand);
            CopyAddressCommand = new Command(copyAddressCommand);
            _vpoint = new ViewRoutePoint(routeId, routePointId);
            Analytics.TrackEvent("Dialog point opened");
            _newPoint = string.IsNullOrEmpty(routePointId);
        }

        private async void updateAddressCommand(object obj)
        {
            if ((Latitude != 0)&& (Longitude != 0))
            {
                await fillAddressAndPointName(Latitude, Longitude);
                ApplyChanges();
            }
        }

        private void cancelNameCommand(object obj)
        {
            IsVisibleModalNameEdit = !IsVisibleModalNameEdit;
        }

        private void clearNameCommand(object obj)
        {
            NameForEdit = string.Empty;
        }

        private void editNameCompleteCommand(object obj)
        {
            if (!Name.Equals(_nameForEdit))
            {
                Name = _nameForEdit;
                ApplyChanges();
            }
            _nameForEdit = string.Empty;
            IsVisibleModalNameEdit = !IsVisibleModalNameEdit;
        }

        private void editNameCommand(object obj)
        {
            IsVisibleModalNameEdit = !IsVisibleModalNameEdit;
            if (IsVisibleModalNameEdit)
            {
                NameForEdit = _vpoint.Name;
            }
        }

        private void backNavigationCommand(object obj)
        {
            Navigation.PopModalAsync();
        }
        private async void copyAddressCommand(object obj)
        {
            await Clipboard.SetTextAsync(Address);
            MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = CommonResource.RoutePoint_AddressCopiedToClipboard }, string.Empty);
        }

        private async void copyCoordinatesCommand(object obj)
        {
            await Clipboard.SetTextAsync(Coordinates);
            MessagingCenter.Send<UIToastMessage>(new UIToastMessage() { Delay = 3, Message = CommonResource.RoutePoint_GeoCoordinatesCopiedToClipboard }, string.Empty);
        }

        private async void editDescriptionCommand(object obj)
        {
            _vpoint.Save();
            await Navigation.PushModalAsync(new EditRoutePointDescriptionPage(_vpoint.Id));
        }

        private void shareCommand(object obj)
        {
            ICommonShareService commonShareService = DependencyService.Get<ICommonShareService>();
            commonShareService.Share(_vpoint, string.Empty);
        }

        private async void addAudioAsync(object obj)
        {
            _vpoint.Version++;
            _vpoint.Save();
            PermissionManager permissions = new PermissionManager();
            if (await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Microphone, CommonResource.RoutePoint_RightNeedToRecordAudio))
            {
                string mediaId = Guid.NewGuid().ToString();
                string pathAudioFile = Path.Combine(ImagePathManager.GetPicturesDirectory(), "audio_" + mediaId + ".3gp");

                AudioManager audioManager = new AudioManager();
                bool resultRecordAudio = await audioManager.RecordAsync(pathAudioFile);
                if (resultRecordAudio)
                {
                    _vpoint.AddMediaItem(mediaId, MediaObjectTypeEnum.Audio);
                    ApplyChanges();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OneImagePath"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
                    Analytics.TrackEvent("Media: audio recorded");
                }
            }
        }

        private async void addPhotoAsync(object obj)
        {
            ApplyChanges();
            ImageManager imageManager = new ImageManager();
            var pickPhotoResult = await imageManager.PickPhotoAsync();
            if (pickPhotoResult.pickPhotoResult)
            {
                if ((pickPhotoResult.imageGpsCoordinates.Latitude > 0 && pickPhotoResult.imageGpsCoordinates.Longitude > 0) && await App.Current.MainPage.DisplayAlert(CommonResource.RoutePoint_GeotagsExists,
                        CommonResource.RoutePoint_UseGeotagsForPoint, CommonResource.CommonMsg_Yes, CommonResource.CommonMsg_No))
                {
                    _vpoint.Address = string.Empty;
                    Latitude = pickPhotoResult.imageGpsCoordinates.Latitude;
                    Longitude = pickPhotoResult.imageGpsCoordinates.Longitude;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Coordinates"));
                    ApplyChanges();
                    await fillAddressAndPointName(Latitude, Longitude);
                    ApplyChanges();

                }
                _vpoint.AddMediaItem(pickPhotoResult.newMediaId, MediaObjectTypeEnum.Image);
                ApplyChanges();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsOneImagesPresent"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OneImagePath"));
                Analytics.TrackEvent("Media: photo added");
            }
        }

        private async void deletePhotoAsync(object imageSource)
        {
            string mediaId = string.Empty;
            if (imageSource is MediaPreview)
            {
                mediaId = ((MediaPreview)imageSource).MediaId;
            }
            else
            {
                if (Images.Count() > 0)
                {
                    mediaId = Images.FirstOrDefault()?.MediaId;
                }
            }
            if (!string.IsNullOrEmpty(mediaId))
            {
                bool delete = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Message = CommonResource.RoutePoint_AreYouSureToDeletePhoto, Title = CommonResource.RoutePoint_DeletingPhoto, OkText = CommonResource.CommonMsg_Yes, CancelText = CommonResource.CommonMsg_No });
                if (delete)
                {
                    _vpoint.DeleteImage(mediaId);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsOneImagesPresent"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OneImagePath"));
                }
            }
        }
        private async void deletePoint(object obj)
        {
            bool delete = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Message = CommonResource.RoutePoint_AreYouSureToDeletePoint, Title = CommonResource.RoutePoint_DeletingPoint, OkText = CommonResource.CommonMsg_Yes, CancelText = CommonResource.CommonMsg_No });
            if (delete)
            {
                if (_vpoint.SetDeleteMarkPointWithDeleteMedias())
                {
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await UserDialogs.Instance.AlertAsync(CommonResource.RoutePoint_ErrorWhileDeletingPoint, CommonResource.CommonMsg_Warning);
                    HandleError.Process("RoutePoint", "DeletePoint", new Exception($"PointId:{_vpoint.Id}, Name:{_vpoint.Name}"), false);
                }
            }
        }

        private void playMediaAsync(object mediaSource)
        {
            var defaultViewerService = DependencyService.Get<IDefaultViewer>();
            defaultViewerService.Show((string)mediaSource);
        }

        private void viewPhotoAsync(object imageSource)
        {
            string path = string.Empty;
            var defaultViewerService = DependencyService.Get<IDefaultViewer>();
            if(imageSource is MediaPreview)
            {
                path = ((MediaPreview)imageSource).SourceImg;
            }
            else
            {
                if (Images.Count() > 0)
                {
                    path = Images.FirstOrDefault()?.SourceImg;
                }
            }
            if (!string.IsNullOrEmpty(path))
            {
                defaultViewerService.Show(path.Replace("_preview", ""));
            }
        }

        private async void takePhotoAsync(object obj)
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                if (string.IsNullOrEmpty(_vpoint.Id))
                {
                    ApplyChanges();
                }

                ImageManager imageManager = new ImageManager();
                var takePhotoResult = await imageManager.TakePhotoAsync(_vpoint.Latitude, _vpoint.Longitude);
                if(takePhotoResult.result)
                {
                    ImagePreviewManager preview = new ImagePreviewManager();
                    if (preview.CreateImagePreview(takePhotoResult.newMediaId))
                    {
                        _vpoint.AddMediaItem(takePhotoResult.newMediaId, MediaObjectTypeEnum.Image);
                        ApplyChanges();
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsOneImagesPresent"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OneImagePath"));
                        Analytics.TrackEvent("Media: photo taken");
                    }
                    else
                    {
                        Analytics.TrackEvent("Media: new photo error create preview", new Dictionary<string, string> { { "mediaId", takePhotoResult.newMediaId } });
                    }
                }
                else
                {
                    Analytics.TrackEvent("Media: new take photo error", new Dictionary<string, string> { { "mediaId", takePhotoResult.newMediaId } });
                }
            }
        }

        public void ApplyChanges()
        {
            _vpoint.Version++;
            _vpoint.Save();
        }

        public IEnumerable<MediaPreview> Images
        {
            get
            {
                var list = _vpoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new MediaPreview() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType }).ToList();
                return list;
            }
        }

        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(_vpoint.Description))
                    return _vpoint.Description;
                else return CommonResource.RoutePoint_DescriptionAbsent;
            }
            set
            {
                _vpoint.Description = value;
            }
        }

        public string Address
        {
            set
            {
                if (_vpoint.Address != value)
                {
                    _vpoint.Address = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Address"));
                }
            }
            get
            {
                return string.IsNullOrEmpty(_vpoint.Address)? CommonResource.RoutePoint_AddressAbsent:_vpoint.Address;
            }
        }

        public double Latitude
        {
            set
            {
                if (_vpoint.Latitude != value)
                {
                    _vpoint.Latitude = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Latitude"));
                }
            }
            get
            {
                return _vpoint.Latitude;
            }
        }
        public double Longitude
        {
            set
            {
                if (_vpoint.Longitude != value)
                {
                    _vpoint.Longitude = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Longitude"));
                }
            }
            get
            {
                return _vpoint.Longitude;
            }
        }

        public bool IsVisibleModalNameEdit
        {
            set
            {
                if(_isVisibleModalNameEdit != value)
                {
                    _isVisibleModalNameEdit = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisibleModalNameEdit"));
                }
            }
            get
            {
                return _isVisibleModalNameEdit;
            }
        }
        /// <summary>
        /// В случае если медиа нет - это тоже одна картинка, поскольку показать надо будет заглушку
        /// </summary>
        public bool IsOneImagesPresent
        {
            get
            {
                int count = Images.Count();
                return count == 0 || count == 1;
            }
        }

        /// <summary>
        /// Возвращает картинку если она одна в данной точке, либо заглушку для пустого фото
        /// </summary>
        public string OneImagePath
        {
            get
            {
                string pathToSingleImage = "emptyphoto.png";
                var listObjects = _vpoint.MediaObjects.Where(x => !x.IsDeleted).ToList();
                if(listObjects.Count == 1)
                {

                    pathToSingleImage = ImagePathManager.GetImagePath(listObjects[0].RoutePointMediaObjectId, (MediaObjectTypeEnum)listObjects[0].MediaType, true);
                }
                return pathToSingleImage;
            }
        }

        public string Coordinates
        {
            get
            {
                return Latitude + "," + Longitude;
            }
        }
        public string Name
        {
            set
            {
                if (_vpoint.Name != value)
                {
                    _vpoint.Name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return string.IsNullOrEmpty(_vpoint.Name)? CommonResource.RoutePoint_NameAbsent : _vpoint.Name;
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

        public async void StartDialog()
        {
            _vpoint.Refresh(_vpoint.Id);
            //Пока не знаю как поймать событие того, редактировалось описание на другой странице и вернулись на текущую уже с модифицированным описанием
            //Ловля события не помогает, поскольку ловится предидущим активити...
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
            MessagingCenter.Subscribe<RoutePointDescriptionModifiedMessage>(this, string.Empty, (sender) =>
            {
                if (sender.RoutePointId.Equals(_vpoint.Id))
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                }
            });
            MessagingCenter.Subscribe<MapSelectNewPointMessage>(this, string.Empty, (sender) =>
            {
                setNewCoordinates(sender.Latitude, sender.Longitude);
            });

            if ((_newPoint) && (_vpoint.Latitude == 0) && (_vpoint.Longitude == 0))
            {

                var cacheCoordinates = await _geolocatorManager.GetLastKnownPosition();
                if ((cacheCoordinates.Latitude != 0) && (cacheCoordinates.Longtitude != 0))
                {
                    Latitude = cacheCoordinates.Latitude;
                    Longitude = cacheCoordinates.Longtitude;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Coordinates"));
                }

                var coordinates = await _geolocatorManager.GetCurrentLocationAsync();
                if ((coordinates.Latitude != 0) && (coordinates.Longtitude != 0))
                {
                    Latitude = coordinates.Latitude;
                    Longitude = coordinates.Longtitude;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Coordinates"));
                    ApplyChanges();
                    await fillAddressAndPointName(Latitude, Longitude);
                    ApplyChanges();
                }

            }

            Device.StartTimer(TimeSpan.FromMilliseconds(100), OnTimerForUpdateLocation);
        }

        private bool OnTimerForUpdateLocation()
        {
            MessagingCenter.Send<MapUpdateLocationPointMessage>(new MapUpdateLocationPointMessage() { }, string.Empty);
            return false;
        }

        private async Task fillAddressAndPointName(double latitude, double longitude)
        {
            var address = await _geolocatorManager.GetPositionAddress(new Plugin.Geolocator.Abstractions.Position(latitude, longitude));
            if (!string.IsNullOrEmpty(address.PositionAddress))
            {
                _vpoint.Address = address.PositionAddress;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Address"));
            }
            if (!string.IsNullOrEmpty(address.PointName) && (string.IsNullOrEmpty(_vpoint.Name)))
            {
                _vpoint.Name = address.PointName;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public void CloseDialog()
        {
            MessagingCenter.Unsubscribe<RoutePointDescriptionModifiedMessage>(this, string.Empty);
            MessagingCenter.Unsubscribe<MapSelectNewPointMessage>(this, string.Empty);
        }

        private async void setNewCoordinates(double latitude, double longitude)
        {
            var notCancel = await Application.Current.MainPage.DisplayAlert(CommonResource.CommonMsg_Warning, CommonResource.RoutePoint_AreYouSureToSetNewCoordinates, CommonResource.CommonMsg_No, CommonResource.CommonMsg_Yes);
            if (!notCancel)
            {
                if((latitude != Latitude) || (longitude != Longitude))
                {
                    Latitude = latitude;
                    Longitude = longitude;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Coordinates"));
                    ApplyChanges();
                    await fillAddressAndPointName(Latitude, Longitude);
                    ApplyChanges();
                }
            }
        }

        public class MediaPreview
        {
            public string SourceImg { get; set; }
            public string MediaId;
            public MediaObjectTypeEnum MediaType;
        }

    }
}
