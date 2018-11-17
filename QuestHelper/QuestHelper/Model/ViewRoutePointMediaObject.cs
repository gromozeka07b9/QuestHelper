using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Model
{
    public class ViewRoutePointMediaObject
    {
        private string _id = string.Empty;
        private string _filename = string.Empty;
        private string _filenamePreview = string.Empty;
        private string _routePointId = string.Empty;
        private int _version = 0;

        public ViewRoutePointMediaObject()
        {
        }

        public void Load(string mediaId)
        {
            RoutePointMediaObjectManager manager = new RoutePointMediaObjectManager();
            var mediaObject = manager.GetMediaObjectById(mediaId);
            if (mediaObject != null)
            {
                _id = mediaObject.RoutePointMediaObjectId;
                _filename = mediaObject.FileName;
                _filenamePreview = mediaObject.FileNamePreview;
            }
        }

        public void Refresh(string mediaId)
        {
            if(!string.IsNullOrEmpty(mediaId))
            {
                Load(mediaId);
            }
        }
        public string Id
        {
            set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }
        public string RoutePointMediaObjectId
        {
            set
            {
                _id = value;
            }
            get
            {
                return _id;
            }
        }

        public string FileName
        {
            get
            {
                return _filename;
            }
            set
            {
                _filename = value;
            }
        }
        public string FileNamePreview
        {
            get
            {
                return _filenamePreview;
            }
            set
            {
                _filenamePreview = value;
            }
        }
        public string RoutePointId
        {
            get
            {
                return _routePointId;
            }
            set
            {
                _routePointId = value;
            }
        }
        public int Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;
            }
        }

        internal bool Save()
        {
            RoutePointMediaObjectManager manager = new RoutePointMediaObjectManager();
            _id = manager.Save(this);
            return !string.IsNullOrEmpty(_id);
        }
    }
}
