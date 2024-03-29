using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using QuestHelper.Consts;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.View;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace QuestHelper.ViewModel
{
    public class MapRouteOverviewV2ViewModel : INotifyPropertyChanged, IDialogEvents
    {
        private readonly string _routeId;
        private readonly TrackFileManager _trackFileManager;
        private readonly RoutePointManager _routePointManager;
        private bool _isRoutePointDialogVisible = false;
        private ViewRoutePoint _selectedRoutePoint = new ViewRoutePoint();
        private bool _setNewLocationMode = false;
        private bool _isVisibleTextForSetNewLocation = false;
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        
        public ICommand HidePoiDialogCommand { get; private set; }
        public ICommand OpenRoutePointDialogCommand { get; private set; }
        public ICommand SetNewLocationCommand { get; private set; }
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand CancelSetLocationCommand { get; private set; }

        public MapRouteOverviewV2ViewModel(string routeId)
        {
            HidePoiDialogCommand = new Command(hidePoiDialogCommand);
            OpenRoutePointDialogCommand = new Command(openRoutePointDialogCommand);
            SetNewLocationCommand = new Command(setNewLocationCommand);
            BackNavigationCommand = new Command(backNavigationCommandAsync);
            CancelSetLocationCommand = new Command(cancelSetLocationCommand);
            _routeId = routeId;
            _trackFileManager = new TrackFileManager();
            _routePointManager = new RoutePointManager();
            RoutePointFrameWidth = Convert.ToInt32(DeviceSize.FullScreenWidth * 0.9);
            RoutePointFrameHeight = Convert.ToInt32(DeviceSize.FullScreenHeight * 0.7);

        }

        private async void backNavigationCommandAsync(object obj)
        {
            await Navigation.PopModalAsync();
        }

        private void cancelSetLocationCommand(object obj)
        {
            SetNewLocationMode = false;
        }
        
        private void setNewLocationCommand(object obj)
        {
            _setNewLocationMode = true;
            IsRoutePointDialogVisible = false;
            IsVisibleTextForSetNewLocation = _setNewLocationMode;
        }

        public bool SetNewLocationMode
        {
            get => _setNewLocationMode;
            set
            {
                if (value != _setNewLocationMode)
                {
                    _setNewLocationMode = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SetNewLocationMode"));
                    IsVisibleTextForSetNewLocation = _setNewLocationMode;
                }
            }
        }

        public bool IsVisibleTextForSetNewLocation
        {
            get => _isVisibleTextForSetNewLocation;
            set
            {
                if (value != _isVisibleTextForSetNewLocation)
                {
                    _isVisibleTextForSetNewLocation = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisibleTextForSetNewLocation"));
                }
            }
        }

        public void SetNewLocation(Position position)
        {
            if (!string.IsNullOrEmpty(SelectedRoutePoint.Id) && SetNewLocationMode)
            {
                SelectedRoutePoint.Latitude = position.Latitude;
                SelectedRoutePoint.Longitude = position.Longitude;
                SelectedRoutePoint.Version++;
                SelectedRoutePoint.Save();
                SetNewLocationMode = false;
                MessagingCenter.Send<MapUpdateLocationPointMessage>(new MapUpdateLocationPointMessage()
                {
                    RoutePointId = SelectedRoutePoint.Id,
                    Latitude = SelectedRoutePoint.Latitude,
                    Longitude = SelectedRoutePoint.Longitude
                }, string.Empty);
            }
        }

        private void openRoutePointDialogCommand(object obj)
        {
            IsRoutePointDialogVisible = false;
            Navigation.PushModalAsync(new RoutePointV2Page(_routeId, SelectedRoutePoint.Id));
        }

        private void hidePoiDialogCommand(object obj)
        {
            IsRoutePointDialogVisible = false;
        }

        public IEnumerable<Tuple<double?, double?>> GetTrackPlaces()
        {
            return _trackFileManager.GetTrackByRoute(_routeId);
        }

        public IEnumerable<ViewRoutePoint> GetRoutePoints()
        {
            return _routePointManager.GetPointsByRouteId(_routeId);
        }
        
        public void StartDialog()
        {
        }

        public void CloseDialog()
        {

        }

        public void SelectRoutePointPin(string routePointId)
        {
            IsRoutePointDialogVisible = true;
            SelectedRoutePoint = new ViewRoutePoint(_routeId, routePointId);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsNotEmptySelectedRoutePointName"));
        }

        public ViewRoutePoint SelectedRoutePoint
        {
            get
            {
                return _selectedRoutePoint;
            }
            set
            {
                _selectedRoutePoint = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRoutePoint"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRoutePointImage"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRoutePointName"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SelectedRoutePointDescription"));
            }
        }

        public string SelectedRoutePointImage => SelectedRoutePoint?.ImagePreviewPath?? String.Empty;
        public string SelectedRoutePointName => SelectedRoutePoint?.Name?? String.Empty;

        public bool IsNotEmptySelectedRoutePointName
        {
            get
            {
                return !string.IsNullOrEmpty(SelectedRoutePointName);
            }
        }
        public string SelectedRoutePointDescription => SelectedRoutePoint?.Description?? String.Empty;
        public int RoutePointFrameHeight { get; set; }
        public int RoutePointFrameWidth { get; set; }

        public bool IsRoutePointDialogVisible
        {
            get
            {
                return _isRoutePointDialogVisible;
            }
            set
            {
                if (value != _isRoutePointDialogVisible)
                {
                    _isRoutePointDialogVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsRoutePointDialogVisible"));    
                }
            }
        }
    }
}