using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.SharedModelsWS;
using Realms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Model
{
    public class ViewRoutePointMediaObject : ISaveable
    {
        private string _id = string.Empty;
        private string _filename = string.Empty;
        private string _filenamePreview = string.Empty;
        private string _routePointId = string.Empty;
        private bool _originalServerSynced = false;
        private bool _previewServerSynced = false;
        private bool _isDeleted = false;
        private DateTimeOffset _serverSyncedDate;
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
                _routePointId = mediaObject.RoutePointId;
                _filename = $"img_{_id}.jpg";
                _filenamePreview = $"img_{_id}_preview.jpg";
                _version = mediaObject.Version;
                _originalServerSynced = mediaObject.OriginalServerSynced;
                _previewServerSynced = mediaObject.PreviewServerSynced;
                _serverSyncedDate = mediaObject.ServerSyncedDate;
                _isDeleted = mediaObject.IsDeleted;
            }
        }
        internal void FillFromWSModel(SharedModelsWS.RoutePointMediaObject mediaObject)
        {
            if (mediaObject != null)
            {
                _id = mediaObject.Id;
                _routePointId = mediaObject.RoutePointId;
                _filename = $"img_{_id}.jpg";
                _filenamePreview = $"img_{_id}_preview.jpg";
                _version = mediaObject.Version;
                _isDeleted = mediaObject.IsDeleted;
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
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
            set
            {
                _isDeleted = value;
            }
        }
        public bool OriginalServerSynced
        {
            get
            {
                return _originalServerSynced;
            }
            set
            {
                _originalServerSynced = value;
            }
        }
        public bool PreviewServerSynced
        {
            get
            {
                return _previewServerSynced;
            }
            set
            {
                _previewServerSynced = value;
            }
        }
        public DateTimeOffset ServerSyncedDate
        {
            get
            {
                return _serverSyncedDate;
            }
            set
            {
                _serverSyncedDate = value;
            }
        }

        public bool Save()
        {
            RoutePointMediaObjectManager manager = new RoutePointMediaObjectManager();
            _id = manager.Save(this);
            return !string.IsNullOrEmpty(_id);
        }
    }
}
