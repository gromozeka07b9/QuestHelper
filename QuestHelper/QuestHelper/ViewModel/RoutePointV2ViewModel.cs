using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class RoutePointV2ViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand DeleteCommand { get; internal set; }
        public ICommand TakePhotoCommand { get; private set; }
        public ICommand ViewPhotoCommand { get; private set; }
        public ICommand PlayMediaCommand { get; private set; }
        public ICommand DeletePhotoCommand { get; private set; }
        public ICommand DeletePointCommand { get; private set; }
        public ICommand EditDescriptionCommand { get; private set; }
        public ICommand CopyCoordinatesCommand { get; private set; }
        public ICommand AddPhotoCommand { get; private set; }
        public ICommand AddAudioCommand { get; private set; }
        public ICommand ShareCommand { get; private set; }

        ViewRoutePoint _vpoint;
        string _currentPositionString = string.Empty;

        public RoutePointV2ViewModel(string routeId, string routePointId)
        {
            TakePhotoCommand = new Command(takePhotoAsync);
            ViewPhotoCommand = new Command(viewPhotoAsync);
            PlayMediaCommand = new Command(playMediaAsync);
            DeletePhotoCommand = new Command(deletePhotoAsync);
            DeletePointCommand = new Command(deletePoint);
            AddPhotoCommand = new Command(addPhotoAsync);
            AddAudioCommand = new Command(addAudioAsync);
            ShareCommand = new Command(shareCommand);
            EditDescriptionCommand = new Command(editDescriptionCommand);
            CopyCoordinatesCommand = new Command(copyCoordinatesCommand);
            _vpoint = new ViewRoutePoint(routeId, routePointId);
        }

        private void copyCoordinatesCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private void editDescriptionCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private void shareCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private void addAudioAsync(object obj)
        {
            throw new NotImplementedException();
        }

        private void addPhotoAsync(object obj)
        {
            throw new NotImplementedException();
        }

        private void deletePhotoAsync(object obj)
        {
            throw new NotImplementedException();
        }
        private void deletePoint(object obj)
        {
            throw new NotImplementedException();
        }

        private void playMediaAsync(object obj)
        {
            throw new NotImplementedException();
        }

        private void viewPhotoAsync(object obj)
        {
            throw new NotImplementedException();
        }

        private void takePhotoAsync(object obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MediaPreview> Images
        {
            get
            {
                var list = _vpoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new MediaPreview() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType }).ToList();
                return list;
            }
        }
        public IEnumerable<ViewRoutePointMediaObject> Images3
        {
            get
            {
                List<ViewRoutePointMediaObject> viewImages = new List<ViewRoutePointMediaObject>();
                var list = _vpoint.MediaObjects.Where(x => !x.IsDeleted);
                foreach(var item in list)
                {
                    var img = new ViewRoutePointMediaObject();
                    img.Load(img.Id);
                    viewImages.Add(img);
                }
                return viewImages;
            }
        }

        public string[] Images2
        {
            get
            {
                return new string[] { "emptyimg.png", "emptyimg.png", "emptyimg.png" };
            }
        }

        public string Description
        {
            get
            {
                if (!string.IsNullOrEmpty(_vpoint.Description))
                    return _vpoint.Description;
                else return CommonResource.RoutePoint_DescriptionAbsent;
            }
            set
            {
                _vpoint.Description = value;
            }
        }

        public string Address
        {
            set
            {
                if (_vpoint.Address != value)
                {
                    _vpoint.Address = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Address"));
                    //ApplyChanges();
                }
            }
            get
            {
                return _vpoint.Address;
            }
        }

        public string Coordinates
        {
            set
            {
                if (_currentPositionString != value)
                {
                    _currentPositionString = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Coordinates"));
                }
            }
            get
            {
                return _currentPositionString;
            }
        }
        public string Name
        {
            set
            {
                if (_vpoint.Name != value)
                {
                    _vpoint.Name = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
                }
            }
            get
            {
                return _vpoint.Name;
            }
        }


        public void StartDialog()
        {
        }

        public void CloseDialog()
        {
        }

        public class MediaPreview
        {
            public string SourceImg { get; set; }
            public string MediaId;
            public MediaObjectTypeEnum MediaType;
        }

    }
}
