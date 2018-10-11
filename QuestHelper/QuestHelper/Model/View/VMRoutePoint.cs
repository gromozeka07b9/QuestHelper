using QuestHelper.LocalDB.Model;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Model.View
{
    public class VMRoutePoint
    {
        private RoutePoint _dBRoutePoint;
        private ImageSource _imagePreview;
        public VMRoutePoint(RoutePoint dbRoutePoint)
        {
            _dBRoutePoint = dbRoutePoint;
        }

        public string Id
        {
            get
            {
                return _dBRoutePoint.RoutePointId;
            }
        }
        public string Name
        {
            get
            {
                return _dBRoutePoint.Name;
            }
        }
        public string CreateDateText
        {
            get
            {
                return _dBRoutePoint.CreateDate.ToLocalTime().ToString();
            }
        }
        public string Description
        {
            get
            {
                return _dBRoutePoint.Description;
            }
        }
        public ImageSource ImagePreview
        {
            get
            {
                if (_dBRoutePoint.MediaObjects.Count > 0)
                {
                    return ImageSource.FromFile(_dBRoutePoint.MediaObjects[0].FileNamePreview);
                } else
                {
                    return ImageSource.FromFile("earth21.png");
                }
            }
        }
    }
}
