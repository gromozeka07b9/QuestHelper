﻿using QuestHelper.Managers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace QuestHelper.Model
{
    public class AutoGeneratedRouted
    {
        IImageManager _imageManager;
        List<AutoGeneratedPoint> _points = new List<AutoGeneratedPoint>();

        public AutoGeneratedRouted(IImageManager imageManager)
        {
            _imageManager = imageManager;
        }

        public string Name { get; internal set; }
        public IEnumerable<GalleryImage> SourceGalleryImages { get; internal set; }

        /*internal Dictionary<(GpsCoordinates, DateTime), GalleryImage> LoadMetadataFromImages()
        {
            var groupedPhotos = new Dictionary<(GpsCoordinates, DateTime), GalleryImage>();
            foreach (var image in SourceGalleryImages)
            {
                var resultImage = _imageManager.GetPhoto(image.ImagePath);
                groupedPhotos.Add((resultImage.imageGpsCoordinates, image.CreateDate), image);
            }
            return groupedPhotos;
        }*/

        internal void Build(Dictionary<(GpsCoordinates, DateTime), GalleryImage> groupedPhotos)
        {
            _points = new List<AutoGeneratedPoint>();
            var sortedByTime = groupedPhotos.OrderBy(x => x.Key.Item2);

            var groupByDate = new List<List<(GpsCoordinates, DateTime, GalleryImage)>>();
            groupByDate.Add(new List<(GpsCoordinates, DateTime, GalleryImage)>());
            var date = sortedByTime.First().Key.Item2;
            int number = 0;
            foreach (var image in sortedByTime)
            {
                /*if (image.Key.Item2.TimeOfDay.TotalSeconds-date.TimeOfDay.TotalSeconds < 1000)
                {
                    groupByDate[number].Add((image.Key.Item1, image.Key.Item2, image.Value));


                }
                else*/
                {
                    AutoGeneratedPoint point = new AutoGeneratedPoint() { Name = (number + 1).ToString(), CreateDate = date, Longitude = image.Key.Item1.Longitude, Latitude = image.Key.Item1.Latitude };//Добавить широту и долготу
                    /*foreach(var groupedImage in groupByDate[number].Select(x => x.Item3.ImagePath))
                    {
                        point.Images.Add(groupedImage);
                    }*/
                    
                    _points.Add(point);
                    groupByDate.Add(new List<(GpsCoordinates, DateTime, GalleryImage)>());
                    date = image.Key.Item2;
                    number++;
                    groupByDate[number] = new List<(GpsCoordinates, DateTime, GalleryImage)>();
                    groupByDate[number].Add((image.Key.Item1, image.Key.Item2, image.Value));
                }
            }


            //double longitude = 0.0;
            //double latitude = 0.0;
            //foreach (var image in SourceGalleryImages)
            //{
            //    var resultImage = imageManager.GetPhoto(image.ImagePath);
            //    if (resultImage.getMetadataPhotoResult)
            //    {
            //        var differenceKm = Location.CalculateDistance(latitude, longitude, resultImage.imageGpsCoordinates.Latitude, resultImage.imageGpsCoordinates.Longitude, DistanceUnits.Kilometers);
            //        longitude = resultImage.imageGpsCoordinates.Longitude;
            //        latitude = resultImage.imageGpsCoordinates.Latitude;
            //        AutoGeneratedPoint point = new AutoGeneratedPoint() { Name = image.CreateDate.ToString(), CreateDate = image.CreateDate, Longitude = longitude, Latitude = latitude };
            //        _points.Add(point);
            //    }
            //}
        }

        internal void BuildOld(List<ViewLocalFile> listCachedImages)
        {
            _points = new List<AutoGeneratedPoint>();
            var sortedByTime = listCachedImages.OrderBy(x => x.FileNameDate);

            double longitude = 0.0;
            double latitude = 0.0;
            AutoGeneratedPoint point = null;
            foreach (var image in sortedByTime)
            {
                if (image.Processed) 
                {
                    var differenceKm = Location.CalculateDistance(latitude, longitude, image.Latitude, image.Longitude, DistanceUnits.Kilometers);
                    longitude = image.Longitude;
                    latitude = image.Latitude;
                    if ((differenceKm > 0.4) || (point == null))
                    {
                        point = new AutoGeneratedPoint() { Name = "Точка маршрута", CreateDate = image.FileNameDate.DateTime, Longitude = longitude, Latitude = latitude };
                        _points.Add(point);
                    }
                    //point.Images.Add(image.ImagePreviewFileName);
                    point.Images.Add(new AutoGeneratedImage(image));
                }
            }
        }

        public ObservableCollection<AutoGeneratedPoint> Points
        {
            get
            {
                return new ObservableCollection<AutoGeneratedPoint>(_points.Where(p=>!p.IsDeleted));
            }
        }

        public class AutoGeneratedImage : INotifyPropertyChanged
        {
            private string _imagePreviewFileName;
            private ViewLocalFile _viewLocalFile;
            private bool _isDeleted;
            private string _id;

            public AutoGeneratedImage(ViewLocalFile viewLocalFile)
            {
                _viewLocalFile = viewLocalFile;
                _imagePreviewFileName = _viewLocalFile.ImagePreviewFileName;
            }

            public string Id
            {
                get
                {
                    return _id;
                }
                set
                {
                    _id = value;
                    this.RaisedOnPropertyChanged("Id");
                }

            }

            public string ImagePreviewFileName
            {
                get
                {
                    return _imagePreviewFileName;
                }
                set
                {
                    _imagePreviewFileName = value;
                    this.RaisedOnPropertyChanged("ImagePreviewFileName");
                }

            }

            public bool IsDeleted
            {
                get
                {
                    return _isDeleted;
                }
                set
                {
                    _isDeleted = value;
                    this.RaisedOnPropertyChanged("IsDeleted");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void RaisedOnPropertyChanged(string _PropertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
                }
            }
        }

        public class AutoGeneratedPoint : INotifyPropertyChanged
        {
            private bool _isSelected;
            private bool _isDeleted;
            private double _latitude;
            private double _longitude;
            private DateTime _createDate;
            private string _name;
            private ObservableCollection<AutoGeneratedImage> _images;
            public ICommand DeletePreviewPointCommand { get; private set; }

            public AutoGeneratedPoint()
            {
                DeletePreviewPointCommand = new Command(deletePreviewPointCommand);
            }

            private void deletePreviewPointCommand(object obj)
            {
                IsDeleted = true;
            }

            public string Name
            {
                get
                {
                    return _name;
                }
                set
                {
                    _name = value;
                    this.RaisedOnPropertyChanged("Name");
                }
            }

            public DateTime CreateDate
            {
                get
                {
                    return _createDate;
                }
                set
                {
                    _createDate = value;
                    this.RaisedOnPropertyChanged("CreateDate");
                }
            }

            public double Longitude
            {
                get
                {
                    return _longitude;
                }
                set
                {
                    _longitude = value;
                    this.RaisedOnPropertyChanged("Longitude");
                }
            }

            public double Latitude
            {
                get
                {
                    return _latitude;
                }
                set
                {
                    _latitude = value;
                    this.RaisedOnPropertyChanged("Latitude");
                }
            }

            public ObservableCollection<AutoGeneratedImage> Images { get; internal set; } = new ObservableCollection<AutoGeneratedImage>();

            public ObservableCollection<AutoGeneratedImage> ImagesOnlyFirstThree
            {
                get
                {
                    int maxItems = 3;
                    var showedImagesList = Images.Where(i => !i.IsDeleted).Take(maxItems).ToList();
                    if(!showedImagesList.Any())
                    {
                        showedImagesList = Images.Take(maxItems).ToList();
                    }
                    return new ObservableCollection<AutoGeneratedImage>(showedImagesList);
                }
            }

            public bool IsDeleted
            {
                get
                {
                    return _isDeleted;
                }
                set
                {
                    _isDeleted = value;
                    this.RaisedOnPropertyChanged("IsDeleted");
                }
            }

            public bool IsSelected
            {
                get
                {
                    return _isSelected;
                }
                set
                {
                    _isSelected = value;
                    this.RaisedOnPropertyChanged("IsSelected");
                }
            }

            public string PointCoordinatesText
            {
                get
                {
                    return $"{Latitude  },{Longitude}";
                }
            }

            public string CreateDateText
            {
                get
                {
                    return CreateDate.ToString("MMM dd");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public void RaisedOnPropertyChanged(string _PropertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
                }
            }
        }

    }
}