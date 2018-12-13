﻿using Microsoft.AppCenter.Crashes;
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
using Xamarin.Forms;
using QuestHelper.Model;

namespace QuestHelper.ViewModel
{
    class RoutePointViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand DeleteCommand { get; internal set; }
        public ICommand TakePhotoCommand { get; private set; }
        public ICommand EditDescriptionCommand { get; private set; }
        public ICommand CopyCoordinatesCommand { get; private set; }

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
            TakePhotoCommand = new Command(takePhoto);
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
        }

        public void StartDialog()
        {
            _vpoint.Refresh(_vpoint.Id);
            //Пока не знаю как поймать событие того, редактировалось описание на другой странице и вернулись на текущую уже с модифицированным описанием
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
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
        private async void takePhoto(object obj)
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
                //string photoName = $"img_{_vpoint.Id}.jpg";
                string photoNamePreview = ImagePathManager.GetImageFilename(mediaId, true);
                //string photoNamePreview = $"img_{_vpoint.Id}_preview.jpg";
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Full,
                    Location = new Location() { Latitude = _vpoint.Latitude, Longitude = _vpoint.Longitude, Timestamp = DateTime.Now },
                    //Directory = "Photos",
                    Directory = string.Empty,
                    Name = photoName,
                    CompressionQuality = 50
                });
                if (file != null)
                {
                    FileInfo info = new FileInfo(file.Path);
                    string imgPathToDirectory = info.DirectoryName;
                    file.Dispose();
                    ImagePreviewManager preview = new ImagePreviewManager();
                    preview.CreateImagePreview(imgPathToDirectory, info.Name, photoNamePreview);
                    //_imagePreviewFilePath = Path.Combine(imgPathToDirectory, photoNamePreview);
                    //_vpoint.ImagePath = info.FullName;
                    //_vpoint.ImagePreviewPath = _imagePreviewFilePath;
                    //_vpoint.Version++;
                    //_vpoint.Save();
                    _vpoint.AddImage(mediaId);
                    ApplyChanges();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagePreviewPath"));
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
                    _vpoint.Name = value;
                    ApplyChanges();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }                   
            }
            get
            {
                return _vpoint.Name;
            }
        }

        public string ImagePath
        {
            get
            {
                return _vpoint.ImagePath;
            }
        }
        public string ImagePreviewPath
        {
            get
            {
                /*if(string.IsNullOrEmpty(_vpoint.ImagePreviewPath))
                {
                    if (!string.IsNullOrEmpty(_vpoint.ImagePath))
                    {
                        return _vpoint.ImagePath;
                    }
                    else
                    {
                        return "emptyimg.png";
                    }
                }
                else
                {
                    return _vpoint.ImagePreviewPath;
                }*/
                //пока без превью
                if (!string.IsNullOrEmpty(_vpoint.ImagePath))
                {
                    return _vpoint.ImagePath;
                }
                else
                {
                    return "emptyimg.png";
                }
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
        }

        public void ApplyChanges()
        {
            _vpoint.Version++;
            _vpoint.Save();
        }

    }
}