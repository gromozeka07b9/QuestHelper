﻿using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using System;

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
        private MediaObjectTypeEnum _mediaType;
        RoutePointMediaObjectManager manager = new RoutePointMediaObjectManager();
        private string _processResultText;
        private bool _processed;

        public ViewRoutePointMediaObject()
        {
        }

        public void Load(string mediaId)
        {
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
                _mediaType = (MediaObjectTypeEnum)mediaObject.MediaType;
                _processed = mediaObject.Processed;
                _processResultText = mediaObject.ProcessResultText;
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
                _mediaType = (MediaObjectTypeEnum)mediaObject.MediaType;
            }
        }

        public void Refresh()
        {
            if(!string.IsNullOrEmpty(Id))
            {
                Load(Id);
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

        public bool Processed
        {
            get
            {
                return _processed;
            }
            set
            {
                _processed = value;
            }
        }
        public string ProcessResultText
        {
            get
            {
                return _processResultText;
            }
            set
            {
                _processResultText = value;
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
        public MediaObjectTypeEnum MediaType
        {
            get
            {
                return _mediaType;
            }
            set
            {
                _mediaType = value;
            }
        }

        public bool Save()
        {
            RoutePointMediaObjectManager manager = new RoutePointMediaObjectManager();
            _id = manager.Save(this);
            return !string.IsNullOrEmpty(_id);
        }
        internal bool Delete()
        {
            bool result = manager.Delete(this.RoutePointMediaObjectId);
            if (result)
            {
                _id = string.Empty;
                _routePointId = string.Empty;
                _filename = string.Empty;
                _filenamePreview = string.Empty;
            }
            return result;
        }

        private void DeleteMediaFile()
        {
            MediaFileManager fileManager = new MediaFileManager();
            fileManager.Delete(_id, _mediaType);
        }

        internal bool SetDeleteMarkWithDeleteImage()
        {
            bool result = false;
            _isDeleted = true;
            _version++;
            result = Save();
            if (result)
            {
                DeleteMediaFile();
            }
            return result;
        }
    }
}
