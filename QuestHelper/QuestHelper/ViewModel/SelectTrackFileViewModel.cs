using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.Model.Messages;
using QuestHelper.Resources;
using QuestHelper.SharedModelsWS;
using QuestHelper.View;
using QuestHelper.WS;
using Syncfusion.DataSource.Extensions;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class SelectTrackFileViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }
        public ICommand UpdateTracksCommand { get; private set; }
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        TokenStoreService _tokenService = new TokenStoreService();
        private readonly TrackFileManager _trackFileManager = new TrackFileManager();
        private ObservableCollection<TrackFileElement> _trackFileNames;
        private readonly string _routeId;
        private bool _isVisibleProgress;
        public SelectTrackFileViewModel(string routeId)
        {
            _routeId = routeId;
            UpdateTracksCommand = new Command(updateTracksCommand);
            BackNavigationCommand = new Command(backNavigationCommandAsync);
            CancelCommand = new Command(cancelCommandAsync);
            DialogResult = new OperationResult();
        }

        private async void cancelCommandAsync(object obj)
        {
            //bool delete = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Message = CommonResource.Route_DeleteAllTracksFromMessage, Title = CommonResource.CommonMsg_Warning, OkText = CommonResource.CommonMsg_Yes, CancelText = CommonResource.CommonMsg_No });
            bool delete = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig() { Message = "Удалить треки из маршрута?", Title = CommonResource.CommonMsg_Warning, OkText = CommonResource.CommonMsg_Yes, CancelText = CommonResource.CommonMsg_No });
            if (delete)
            {
                string currentUserToken = await _tokenService.GetAuthTokenAsync();
                TrackRouteRequest trackRequest = new TrackRouteRequest(currentUserToken);
                //trackRequest.RemoveAllTracksFromRoute(_routeId);
                _trackFileManager.RemoveAllTracksFromRoute(_routeId);
                await Navigation.PopModalAsync();
            }
        }

        private async void backNavigationCommandAsync(object obj)
        {
            await Navigation.PopModalAsync();
        }

        private void updateTracksCommand()
        {
            TrackFileNames = _trackFileManager.GetTrackFilesFromDirectory().Select(t=>new TrackFileElement()
            {
                Filename = t.Name,
                CreateDate = t.CreationTime
            }).OrderBy(f=>f.Filename).ToObservableCollection();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TrackFileNames"));
        }

        public void StartDialog()
        {
            updateTracksCommand();
        }

        public void CloseDialog()
        {
        }

        private async Task<bool> tryParseAndGetTrackAsync(string filename, string routeId)
        {
            RouteTracking trackResponse = new RouteTracking();
            TokenStoreService tokenService = new TokenStoreService();
            string currentUserToken = await tokenService.GetAuthTokenAsync();
            TrackRouteRequest sendTrackRequest = new TrackRouteRequest(currentUserToken);
            bool sendResult = await sendTrackRequest.SendTrackFileAsync(filename, routeId);
            if (sendResult)
            {
                trackResponse = await sendTrackRequest.GetTrackPlacesAsync(routeId);
                if (trackResponse.Places.Length > 0)
                {
                    var places = trackResponse.Places.Select(p => new ViewTrackPlace()
                    {
                        Id = p.Id,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        Name = p.Name,
                        Description = p.Description,
                        Address = p.Address,
                        Category = p.Category,
                        Distance = p.Distance,
                        DateTimeBegin = !p.DateTimeBegin.Equals(DateTime.MinValue)? new DateTimeOffset(p.DateTimeBegin) : DateTimeOffset.MinValue,
                        DateTimeEnd = !p.DateTimeEnd.Equals(DateTime.MinValue)? new DateTimeOffset(p.DateTimeEnd) : DateTimeOffset.MinValue,
                        Elevation = 0
                    }).ToArray();
                    _trackFileManager.SaveTrack(routeId, places);
                }
            }


            return sendResult && trackResponse.Places.Length > 0;
        }
        public ObservableCollection<TrackFileElement> TrackFileNames
        {
            get
            {
                return _trackFileNames;
            }
            set
            {
                if (value != _trackFileNames)
                {
                    _trackFileNames = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TrackFileNames"));
                }
            }
        }

        public TrackFileElement SelectedReceivedTrackItem
        {
            set
            {
                Task.Run(async () =>
                {
                    IsVisibleProgress = true;
                    DialogResult.Result = await tryParseAndGetTrackAsync(value.Filename, _routeId);
                    IsVisibleProgress = false;
                    await Navigation.PopModalAsync();
                    value = null;
                });
            }
        }

        public OperationResult DialogResult { get; set; }
        
        public bool IsVisibleProgress
        {
            get => _isVisibleProgress;
            set
            {
                if (value != _isVisibleProgress)
                {
                    _isVisibleProgress = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVisibleProgress"));
                }
            }
        }

        public class TrackFileElement
        {
            public string Filename { get; set; }
            public DateTime CreateDate { get; set; }
            public string CreateDateText
            {
                get
                {
                    return CreateDate.ToShortDateString();
                }
            }
        }
    }
}