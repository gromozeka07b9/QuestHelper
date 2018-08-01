using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Model.DB
{
    public class RoutePoint : RealmObject
    {
        [PrimaryKey]
        public string RoutePointId { get; set; } = Guid.NewGuid().ToString();
        public string RouteId { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreateDate { get; set; } = DateTime.Now;
        public DateTimeOffset UpdateDate { get; set; } = DateTime.Now;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public bool IsNew { get; set; } = true;
        public Route MainRoute { get; set; }
        public IList<RoutePointMediaObject> MediaObjects { get; }

        public string CoordinatesByText
        {
            get
            {
                string coordsText = "Not defined";
                if ((Latitude > 0) && (Longitude > 0))
                {
                    coordsText = $"{Latitude},{Longitude}";
                }
                return coordsText;
            }
        }
        public ImageSource ImagePreview
        {
            get
            {
                if (MediaObjects.Count > 0)
                {
                    //фуфу, убрать это из модели!
                    var stream = new MemoryStream(MediaObjects[0].PreviewImage);
                    ImageSource src;
                    try
                    {
                        src = ImageSource.FromStream(() => stream);
                    }
                    catch(Exception exception)
                    {
                        src = StreamImageSource.FromFile("star.png");
                    }
                    return src;
                }
                else
                {
                    return StreamImageSource.FromFile("star.png");
                }
            }
        }
    }
}
