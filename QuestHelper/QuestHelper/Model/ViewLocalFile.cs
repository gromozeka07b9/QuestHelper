using QuestHelper.Managers;
using System;
using Xamarin.Forms.Maps;

namespace QuestHelper.Model
{
    public class ViewLocalFile : ISaveable
    {
        private string _id = string.Empty;
        private DateTimeOffset _createDate;
        private string _address = string.Empty;
        private double _latitude;
        private double _longitude;
        private string _country;
        private DateTimeOffset _fileNameDate;
        private string _sourceFileName;
        private string _imagePreviewFileName;
        private string _sourcePath;

        public ViewLocalFile()
        {
            _id = Guid.NewGuid().ToString();
            CreateDate = DateTimeOffset.Now;
        }
        public ViewLocalFile(string id)
        {
            _id = id;
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

        public DateTimeOffset CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                _createDate = value;
            }
        }


        public string Address
        {
            set
            {
                _address = value;
            }
            get
            {
                return _address;
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
            }
        }



        public string Country
        {
            set
            {
                _country = value;
            }
            get
            {
                return _country;
            }
        }
        public DateTimeOffset FileNameDate
        {
            get
            {
                return _fileNameDate;
            }
            set
            {
                _fileNameDate = value;
            }
        }
        public string ImagePreviewFileName
        {
            set
            {
                _imagePreviewFileName = value;
            }
            get
            {
                return _imagePreviewFileName;
            }
        }
        public string SourceFileName
        {
            set
            {
                _sourceFileName = value;
            }
            get
            {
                return _sourceFileName;
            }
        }
        public string SourcePath
        {
            set
            {
                _sourcePath = value;
            }
            get
            {
                return _sourcePath;
            }
        }

        public bool Save()
        {
            LocalFileCacheManager manager = new LocalFileCacheManager();
            return manager.Save(this);
        }
    }
}
