using Microsoft.AppCenter.Crashes;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using QuestHelper.Managers;
using QuestHelper.Managers.Sync;
using QuestHelper.LocalDB.Model;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ExifLib;
using Xamarin.Forms;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using Microsoft.AppCenter.Analytics;
using System.Collections.ObjectModel;
using Acr.UserDialogs;

namespace QuestHelper.ViewModel
{
    class RoutePointViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand DeleteCommand { get; internal set; }
        public ICommand TakePhotoCommand { get; private set; }
        public ICommand ViewPhotoCommand { get; private set; }
        public ICommand DeletePhotoCommand { get; private set; }
        public ICommand EditDescriptionCommand { get; private set; }
        public ICommand CopyCoordinatesCommand { get; private set; }
        public ICommand AddPhotoCommand { get; private set; }

        private string defaultImageName = "emptyimg.png";
        private static Random _rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
        private List<string> _pointNames = new List<string>()
        {
            "Альфа",
            "Бета",
            "Гамма",
            "Сигма",
            "Земля",
            "Марс",
            "Венера",
            "Сатурн",
            "Меркурий",
            "Плутон",
            "Нептун",
            "Юпитер",
            "Уран",
            "НЛО здесь",
            "Место посадки НЛО",
            "Место наблюдения НЛО",
            "Место крушения НЛО",
            "Лагерь инопланетян",
            "База инопланетян",
            "База Дарта Вейдера",
            "База Принцессы Леи",
            "Королевство Кривых зеркал",
            "Девятый вал",
            "Завтрак на Плутоне",
            "Тихий Дон",
            "Три колодца",
            "Трамвай Желаний",
            "В тридевятом царстве"
        };

        ViewRoutePoint _vpoint;
        string _currentPositionString = string.Empty;
        string _imageFilePath = string.Empty;
        string _imagePreviewFilePath = string.Empty;

        public Func<bool> OnDeletePoint { get; internal set; }

        public RoutePointViewModel(string routeId, string routePointId)
        {
            TakePhotoCommand = new Command(takePhotoAsync);
            ViewPhotoCommand = new Command(viewPhotoAsync);
            DeletePhotoCommand = new Command(deletePhotoAsync);
            AddPhotoCommand = new Command(addPhotoAsync);
            EditDescriptionCommand = new Command(editDescriptionCommand);
            CopyCoordinatesCommand = new Command(copyCoordinatesCommand);
            _vpoint = new ViewRoutePoint(routeId, routePointId);
            if (string.IsNullOrEmpty(routePointId))
            {
                int index = _rnd.Next(0, _pointNames.Count() - 1);
                Name = _pointNames[index];
                //if ((_vpoint.Latitude == 0) && (_vpoint.Longitude == 0))
                FillCurrentPositionAsync();
            }

            Coordinates = Latitude + "," + Longitude;
            Analytics.TrackEvent("Dialog point opened");
        }

        private async void SetNewCoordinates(double latitude, double longitude)
        {
            var notCancel = await Application.Current.MainPage.DisplayAlert("Изменение координат точки", "Вы уверены, что хотите установить новые координаты?", "Нет", "Да");
            if (!notCancel)
            {
                _vpoint.Latitude = latitude;
                _vpoint.Longitude = longitude;
                ApplyChanges();
                Coordinates = Latitude + "," + Longitude;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Longitude"));
            }
        }

        private void viewPhotoAsync(object imageSource)
        {
            var defaultViewerService = DependencyService.Get<IDefaultViewer>();
            string path = (FileImageSource) imageSource;
            if (string.IsNullOrEmpty(path))
            {
                takePhotoAsync();
            }
            else
            {
                defaultViewerService.Show(path.Replace("_preview", ""));
            }
        }
        private async void deletePhotoAsync(object mediaId)
        {
            bool delete = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Message = "Вы уверены что хотите удалить фото?", Title = "Удаление фото", OkText = "Да", CancelText = "Нет" });
            if (delete)
            {
                _vpoint.DeleteImage((string)mediaId);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
            }
        }

        private async void addPhotoAsync(object obj)
        {
            _vpoint.Save();
            bool b = await CrossMedia.Current.Initialize();
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                MediaFile photoPicked = await CrossMedia.Current.PickPhotoAsync();

                if (photoPicked != null)
                {
                    string imgPathDirectory = ImagePathManager.GetPicturesDirectory();
                    string mediaId = Guid.NewGuid().ToString();
                    //используем метод создания превью для того, чтобы сделать основное фото из оригинального, но с уменьшенным качеством

                    ImagePreviewManager resizedOriginal = new ImagePreviewManager();
                    resizedOriginal.PreviewHeight = 0;
                    resizedOriginal.PreviewWidth = 0;
                    resizedOriginal.PreviewQuality = 60;
                    FileInfo originalFileInfo = new FileInfo(photoPicked.Path);

                    if (resizedOriginal.CreateImagePreview(originalFileInfo.DirectoryName, originalFileInfo.Name,
                        imgPathDirectory, ImagePathManager.GetImageFilename(mediaId, false)))
                    {
                        ImagePreviewManager preview = new ImagePreviewManager();
                        if (preview.CreateImagePreview(originalFileInfo.DirectoryName, originalFileInfo.Name,
                            imgPathDirectory, ImagePathManager.GetImageFilename(mediaId, true)))
                        {
                            ExifManager exif = new ExifManager();
                            var coords = exif.GetCoordinates(photoPicked.Path);
                            if ((coords.Latitude > 0 && coords.Longitude > 0) && await App.Current.MainPage.DisplayAlert("Доступны координаты съемки",
                                    "Использовать их для данной точки?", "Да", "Нет"))
                            {
                                Latitude = coords.Latitude;
                                Longitude = coords.Longitude;
                                _vpoint.Address = string.Empty;
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Latitude"));
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Longitude"));
                                FillAddressByCoordinatesAsync(Latitude, Longitude);
                            }
                            _vpoint.AddImage(mediaId);
                            ApplyChanges();
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
                            Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(){ShowErrorMessageIfExist = false}, string.Empty);
                            Analytics.TrackEvent("Media: photo added", new Dictionary<string, string> { { "Photo size", originalFileInfo.Length.ToString() } });
                        }
                        else
                        {
                            Analytics.TrackEvent("Media: add photo error create preview ", new Dictionary<string, string> { { "mediaId", mediaId } });
                        }
                    }
                    else
                    {
                        Analytics.TrackEvent("Media: error resize photo ", new Dictionary<string, string> { { "mediaId", mediaId } });
                    }
                }
            }
        }

        internal void CloseDialog()
        {
            MessagingCenter.Unsubscribe<MapSelectNewPointMessage>(this, string.Empty);
        }

        public void StartDialog()
        {
            _vpoint.Refresh(_vpoint.Id);
            //Пока не знаю как поймать событие того, редактировалось описание на другой странице и вернулись на текущую уже с модифицированным описанием
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
            MessagingCenter.Subscribe<MapSelectNewPointMessage>(this, string.Empty, (sender) =>
            {
                SetNewCoordinates(sender.Latitude, sender.Longitude);
            });
        }

        private void copyCoordinatesCommand(object obj)
        {
            //throw new NotImplementedException();
        }

        internal async void DeletePoint()
        {
            if(_vpoint.Delete())
            {
                await Navigation.PopAsync();
            }
        }

        private async void editDescriptionCommand(object obj)
        {
            _vpoint.Save();
            await Navigation.PushAsync(new EditRoutePointDescriptionPage(_vpoint.Id));
        }
        private async void takePhotoAsync()
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                if (string.IsNullOrEmpty(_vpoint.Id))
                {
                    _vpoint.Save();
                }

                string mediaId = Guid.NewGuid().ToString();
                string photoName = ImagePathManager.GetImageFilename(mediaId);
                string photoNamePreview = ImagePathManager.GetImageFilename(mediaId, true);
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Full,
                    Location = new Location() { Latitude = _vpoint.Latitude, Longitude = _vpoint.Longitude, Timestamp = DateTime.Now },
                    Directory = string.Empty,
                    Name = photoName,
                    SaveToAlbum = true,
                    CompressionQuality = 50
                });
                long fileSize = 0;
                if (file != null)
                {
                    FileInfo info = new FileInfo(file.Path);
                    string imgPathToDirectory = info.DirectoryName;
                    fileSize = info.Length;
                    file.Dispose();

                    ImagePreviewManager preview = new ImagePreviewManager();
                    if (preview.CreateImagePreview(imgPathToDirectory, info.Name, imgPathToDirectory, photoNamePreview))
                    {
                        _vpoint.AddImage(mediaId);
                        ApplyChanges();
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Images"));
                        Xamarin.Forms.MessagingCenter.Send<SyncMessage>(new SyncMessage(){ShowErrorMessageIfExist = false}, string.Empty);
                        Analytics.TrackEvent("Media: photo taken", new Dictionary<string, string> { { "Photo size", fileSize.ToString() } });
                    }
                    else
                    {
                        Analytics.TrackEvent("Media: new photo error create preview", new Dictionary<string, string> { { "mediaId", mediaId } });
                    }
                }
            }
        }

        private async void FillCurrentPositionAsync()
        {
            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            Latitude = position.Latitude;
            Longitude = position.Longitude;
            Coordinates = Latitude + "," + Longitude;
            Address = await GetPositionAddress(locator, position);
        }

        private async void FillAddressByCoordinatesAsync(double latitude, double longitude)
        {
            var locator = CrossGeolocator.Current;
            Address = await GetPositionAddress(locator, new Position(latitude, longitude));
        }

        private async Task<string> GetPositionAddress(IGeolocator locator, Position position)
        {
            string address = string.Empty;
            try
            {
                IEnumerable<Address> addresses = await locator.GetAddressesForPositionAsync(position);
                var addressItem = addresses.FirstOrDefault();
                if (addressItem != null)
                {
                    address = $"{addressItem.SubThoroughfare}, {addressItem.Thoroughfare}, {addressItem.Locality}, {addressItem.CountryName}";
                }
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string> { { "Screen", "RoutePoint" }, { "Action", "GetPositionAddress" } };
                Crashes.TrackError(exception, properties);                
            }
            return address;
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
        public string Coordinates
        {
            set
            {
                if (_currentPositionString != value)
                {
                    _currentPositionString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Coordinates"));
                }
            }
            get
            {
                return _currentPositionString;
            }
        }
        public string Name
        {
            set
            {
                if (_vpoint.Name != value)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _vpoint.Name = value;
                        ApplyChanges();
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                    }
                    else
                    {
                        UserDialogs.Instance.Alert(new AlertConfig(){Title = "Внимание!", Message = "Название точки должно быть заполнено", OkText = "Ok"});
                    }
                }                   
            }
            get
            {
                return _vpoint.Name;
            }
        }

        public string Address
        {
            set
            {
                if(_vpoint.Address != value)
                {
                    _vpoint.Address = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Address"));
                    ApplyChanges();
                }
            }
            get
            {
                return _vpoint.Address;
            }
        }
        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(_vpoint.Description))
                    return _vpoint.Description;
                else return "Описание не указано";
            }
            set
            {
                _vpoint.Description = value;
            }
        }

        public List<ImagePreview> Images
        {
            get
            {
                return _vpoint.MediaObjects.Where(x=>!x.IsDeleted).Select(x => new ImagePreview() {Source = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, true), MediaId = x.RoutePointMediaObjectId}).ToList();
            }
        }
        public void ApplyChanges()
        {
            _vpoint.Version++;
            _vpoint.Save();
        }

        public class ImagePreview
        {
            public string Source;
            public string MediaId;
        }
    }
}
