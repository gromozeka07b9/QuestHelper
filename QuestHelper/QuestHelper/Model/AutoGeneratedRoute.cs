﻿using QuestHelper.Managers;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace QuestHelper.Model
{
    internal class AutoGeneratedRoute
    {
        List<AutoGeneratedPoint> _points = new List<AutoGeneratedPoint>();

        public AutoGeneratedRoute()
        {
        }

        public string Name { get; internal set; }
        public IEnumerable<GalleryImage> SourceGalleryImages { get; internal set; }

        internal void Build()
        {
            _points = new List<AutoGeneratedPoint>();
            ImageManager imageManager = new ImageManager();
            double longitude = 0.0;
            double latitude = 0.0;
            foreach (var image in SourceGalleryImages)
            {
                var resultImage = imageManager.GetPhoto(image.ImagePath);
                if (resultImage.getMetadataPhotoResult)
                {
                    var differenceKm = Location.CalculateDistance(latitude, longitude, resultImage.imageGpsCoordinates.Latitude, resultImage.imageGpsCoordinates.Longitude, DistanceUnits.Kilometers);
                    longitude = resultImage.imageGpsCoordinates.Longitude;
                    latitude = resultImage.imageGpsCoordinates.Latitude;
                    if(differenceKm > 0.5)
                    {
                        AutoGeneratedPoint point = new AutoGeneratedPoint() { Name = image.CreateDate.ToString(), CreateDate = image.CreateDate, Longitude = longitude, Latitude = latitude };
                        point.Images.Add(image.ImagePath);
                        _points.Add(point);
                    }
                }
            }
        }

        public List<AutoGeneratedPoint> Points
        {
            get
            {
                return _points;
            }
        }

        internal class AutoGeneratedPoint
        {
            public AutoGeneratedPoint()
            {
            }

            public string Name { get; set; }
            public DateTime CreateDate { get; internal set; }
            public double Longitude { get; internal set; }
            public double Latitude { get; internal set; }
            public List<string> Images = new List<string>();
        }

    }
}