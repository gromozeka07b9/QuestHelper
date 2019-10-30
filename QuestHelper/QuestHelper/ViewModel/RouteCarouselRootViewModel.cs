﻿using Microsoft.AppCenter.Analytics;
using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class RouteCarouselRootViewModel : INotifyPropertyChanged
    {
        private readonly ViewRoute _routeObject;
        private RouteManager _routeManager = new RouteManager();
        private RoutePointManager _routePointManager = new RoutePointManager();
        private CarouselItem _currentItem;
        private string _routePointId = string.Empty;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public RouteCarouselRootViewModel(string routeId, string routePointId = "")
        {
            _routeObject = new ViewRoute(routeId);
            _routePointId = routePointId;
        }

        public string RouteName
        {
            get { return _routeObject?.Name.ToUpper(); }
        }
        public string RoutePointName
        {
            get { return _currentItem?.RoutePointName.ToUpper(); }
        }
        public string RoutePointDescription
        {
            get
            {
                string text = string.Empty;
                if (string.IsNullOrEmpty(_currentItem?.RoutePointDescription))
                {
                    text = "Описание не заполнено";
                }
                else text = _currentItem?.RoutePointDescription;
                return text;
            }
        }
        public bool DescriptionIsVisible
        {
            get
            {
                return !string.IsNullOrEmpty(_currentItem?.RoutePointDescription);
            }
        }

        public List<PointForMap> PointsOnMap { get; } = new List<PointForMap>();

        public CarouselItem CurrentItem
        {
            get { return _currentItem; }
            set
            {
                if (_currentItem != value)
                {
                    _currentItem = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RoutePointName"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RoutePointDescription"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DescriptionIsVisible"));
                }
            }
        }

        public List<CarouselItem> CarouselPages
        {
            get
            {
                List<CarouselItem> items = new List<CarouselItem>();
                IEnumerable<ViewRoutePoint> points = new List<ViewRoutePoint>();
                if (!string.IsNullOrEmpty(_routePointId))
                {
                    points = _routePointManager.GetPointsByRouteId(_routeObject.RouteId).Where(p=>p.Id.Equals(_routePointId));
                }
                else
                {
                    points = _routePointManager.GetPointsByRouteId(_routeObject.RouteId);
                }
                if (points.Any())
                {
                    foreach (var point in points)
                    {
                        PointsOnMap.Add(new PointForMap() { Name = point.Name, Description = point.Description, PathToPicture = point.ImagePreviewPathForList, Latitude = point.Latitude, Longitude = point.Longitude});
                        if (point.MediaObjects.Where(m=>m.MediaType == 0).Any())
                        {
                            foreach (var media in point.MediaObjects.Where(m => m.MediaType == 0))
                            {
                                items.Add(new CarouselItem(){RouteId = _routeObject.RouteId, RoutePointId = point.RoutePointId, MediaId = media.RoutePointMediaObjectId, ImageSource = ImagePathManager.GetImagePath(media.RoutePointMediaObjectId, MediaObjectTypeEnum.Image, true), RoutePointName = point.NameText, RoutePointDescription = point.Description, Latitude = point.Latitude, Longitude = point.Longitude });
                            }
                        }
                        else
                        {
                            items.Add(new CarouselItem() { RouteId = _routeObject.RouteId, RoutePointId = point.RoutePointId, MediaId = string.Empty, RoutePointName = point.NameText, RoutePointDescription = point.Description, Latitude = point.Latitude, Longitude = point.Longitude });
                        }
                    }
                }
                return items;
            }
        }

        public async void StartDialogAsync()
        {
            TokenStoreService token = new TokenStoreService();
            string _userId = await token.GetUserIdAsync();

            //Автора альбома пока не считаем за просмотр
            if (!_routeObject.CreatorId.Equals(_userId))
            {
                Analytics.TrackEvent("Album opened", new Dictionary<string, string> { { "Album", RouteName } });
            }
        }

        public class CarouselItem : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private Aspect _imageAspect = Aspect.AspectFill;
            private string _imageSource = string.Empty;
            public ICommand ChangeImageAspectCommand { get; private set; }
            public ICommand ViewPhotoCommand { get; private set; }

            public CarouselItem()
            {
                ChangeImageAspectCommand = new Command(changeImageAspectCommand);
                ViewPhotoCommand = new Command(viewPhotoAsync);
            }
            private void changeImageAspectCommand(object obj)
            {
                if (PhotoImageAspect == Aspect.AspectFit)
                {
                    PhotoImageAspect = Aspect.AspectFill;
                }
                else
                {
                    PhotoImageAspect = Aspect.AspectFit;
                }
            }
            private void viewPhotoAsync()
            {
                var defaultViewerService = DependencyService.Get<IDefaultViewer>();
                string path = (FileImageSource)_imageSource;
                if (!string.IsNullOrEmpty(path))
                {
                    string filePath = File.Exists(path) ? path : path.Replace("_preview", "");
                    defaultViewerService.Show(filePath);
                }
            }

            public bool IsFullImage { get; set; }

            public string ImageSource
            {
                get
                {
                    return _imageSource;
                }
                set
                {
                    if (_imageSource != value)
                    {
                        _imageSource = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImageSource"));
                    }
                }
            }
            public string RouteId { get; set; }
            public string RoutePointId { get; set; }
            public string MediaId { get; set; }
            public Aspect PhotoImageAspect
            {
                get
                {
                    return _imageAspect;
                }
                set
                {
                    if (_imageAspect != value)
                    {
                        _imageAspect = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PhotoImageAspect"));
                    }
                }
            }
            public string RoutePointName { get; set; }
            public string RoutePointDescription { get; set; }
            public double Latitude { get; internal set; }
            public double Longitude { get; internal set; }
        }
    }
}
