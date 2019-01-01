using System;
using System.Collections.Generic;
using System.Text;
using ExifLib;
using QuestHelper.Model;

namespace QuestHelper.Managers
{
    public class ExifManager
    {
        public GpsCoordinates GetCoordinates(string imageFileName)
        {
            using (var reader = new ExifReader(imageFileName))
            {
                Double[] latitude, longitude;
                var latitudeRef = "";
                var longitudeRef = "";

                if (reader.GetTagValue(ExifTags.GPSLatitude, out latitude)
                    && reader.GetTagValue(ExifTags.GPSLongitude, out longitude)
                    && reader.GetTagValue(ExifTags.GPSLatitudeRef, out latitudeRef)
                    && reader.GetTagValue(ExifTags.GPSLongitudeRef, out longitudeRef))
                {
                    var longitudeTotal = longitude[0] + longitude[1] / 60 + longitude[2] / 3600;
                    var latitudeTotal = latitude[0] + latitude[1] / 60 + latitude[2] / 3600;

                    return new GpsCoordinates()
                    {
                        Latitude = (latitudeRef == "N" ? 1 : -1) * latitudeTotal,
                        Longitude = (longitudeRef == "E" ? 1 : -1) * longitudeTotal,
                    };
                }

                return new GpsCoordinates()
                {
                    Latitude = 0,
                    Longitude = 0,
                };
            }
        }
    }
}
