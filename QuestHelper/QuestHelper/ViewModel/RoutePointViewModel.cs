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
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    class RoutePointViewModel : INotifyPropertyChanged
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand DeleteCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
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

        RoutePoint _point;
        Route _route;
        string _currentPositionString = string.Empty;
        string _imageFilePath = string.Empty;
        string _imagePreviewFilePath = string.Empty;
        byte[] _imagePreview;

        public RoutePointViewModel(Route route, RoutePoint routePoint)
        {
            _route = route;
            _point = routePoint;
            SaveCommand = new Command(saveRoutePoint);
            DeleteCommand = new Command(deleteRoutePoint);
            TakePhotoCommand = new Command(takePhoto);
            EditDescriptionCommand = new Command(editDescriptionCommand);
            CopyCoordinatesCommand = new Command(copyCoordinatesCommand);
            if ((_point.Latitude == 0)&&(_point.Longitude==0))
                fillCurrentPositionAsync();
            Coordinates = Latitude + "," + Longitude;
        }

        private void copyCoordinatesCommand(object obj)
        {
            //throw new NotImplementedException();
        }

        private async void editDescriptionCommand(object obj)
        {
            await Navigation.PushAsync(new EditRoutePointDescriptionPage(_point));
        }
        private async void takePhoto(object obj)
        {

            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                string fileNameGuid = Guid.NewGuid().ToString();
                string photoName = $"img_{fileNameGuid}.jpg";
                string photoNamePreview = $"img_{fileNameGuid}_preview.jpg";
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Full,
                    Location = new Location() { Latitude = _point.Latitude, Longitude = _point.Longitude, Timestamp = DateTime.Now },
                    Directory = "Photos",
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
                    _imagePreviewFilePath = imgPathToDirectory + "/" + photoNamePreview;
                    ImagePath = info.FullName;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                    //SyncFiles.GetInstance().Start();
                }
            }
        }

        private async void fillCurrentPositionAsync()
        {
            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(10));
            Latitude = position.Latitude;
            Longitude = position.Longitude;
            Coordinates = Latitude + "," + Longitude;
            Address = await getPositionAddress(locator, position);

            int index = _rnd.Next(0, _pointNames.Count() - 1);
            Name = _pointNames[index];
        }

        private async Task<string> getPositionAddress(IGeolocator locator, Position position)
        {
            string address = string.Empty;
            try
            {
                IEnumerable<Address> addresses = await locator.GetAddressesForPositionAsync(position);
                var addressItem = addresses.FirstOrDefault();
                address = $"{addressItem.SubThoroughfare}, {addressItem.Thoroughfare}, {addressItem.Locality}, {addressItem.CountryName}";
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string> { { "Screen", "RoutePoint" }, { "Action", "GetPositionAddress" } };
                Crashes.TrackError(exception, properties);
            }
            return address;
        }

        async void saveRoutePoint()
        {
            if(_point.MainRoute == null)
            {
                _point.MainRoute = _route;
                RoutePointManager manager = new RoutePointManager();
                if (manager.Add(_point, _route))
                {
                    await Navigation.PopAsync();
                    SyncServer.SyncAll();
                    /*await _routesApi.AddRoute(_route);
                    await _routePointsApi.AddRoutePoint(_point);
                    if(_point.MediaObjects.Count > 0)
                    {
                        await _routePointMediaObjectsApi.AddRoutePointMediaObject(_point.MediaObjects[0]);
                    }*/
                } else
                {
                    Crashes.TrackError(new Exception("Error while adding new point"), new Dictionary<string, string> { { "Screen", "RoutePoint" }, { "Action", "SaveRoutePoint" } });
                };
            }
        }

        async void deleteRoutePoint()
        {

        }

        public ImageSource ImageSource
        {
            get
            {
                return StreamImageSource.FromResource("emptyimg.png");
                /*if (File.Exists(ImagePath))
                {
                    return StreamImageSource.FromFile(ImagePath);
                } else
                {
                    return StreamImageSource.FromResource("emptyimg.png");
                }*/
            }
        }
        public double Latitude
        {
            set
            {
                if (_point.Latitude != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _point.Latitude = value;
                        _point.UpdateDate = DateTime.Now;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Latitude"));
                }
            }
            get
            {
                return _point.Latitude;
            }
        }
        public double Longitude
        {
            set
            {
                if (_point.Longitude != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _point.Longitude = value;
                        _point.UpdateDate = DateTime.Now;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Longitude"));
                }
            }
            get
            {
                return _point.Longitude;
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
                RoutePointManager manager = new RoutePointManager();
                manager.SetName(_point, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
            get
            {
                return _point.Name;
            }
        }
        public string ImagePath
        {
            set
            {
                if (_imageFilePath != value)
                {
                    RoutePointMediaObjectManager manager = new RoutePointMediaObjectManager();
                    manager.Add(_point, _imagePreviewFilePath, value);
                    _imageFilePath = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagePath"));
                }
            }
            get
            {
                RoutePointManager manager = new RoutePointManager();
                return manager.GetDefaultImageFilename(_point);
            }
        }
        public string ImagePreviewPath
        {
            get
            {
                RoutePointManager manager = new RoutePointManager();
                return manager.GetDefaultImagePreviewFilename(_point);
            }
        }

        public string Address
        {
            set
            {
                if (_point.Address != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _point.Address = value;
                        _point.UpdateDate = DateTime.Now;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Address"));
                }
            }
            get
            {
                return _point.Address;
            }
        }
        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(_point.Description))
                    return _point.Description;
                else return "Описание не указано";
            }
        }

    }
}
