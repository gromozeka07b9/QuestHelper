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
using QuestHelper.Resources;
using QuestHelper.SharedModelsWS;
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

        private TrackFileManager _trackFileManager = new TrackFileManager();
        private ObservableCollection<TrackFileElement> _trackFileNames;
        private string _routeId;

        public SelectTrackFileViewModel(string routeId)
        {
            _routeId = routeId;
            UpdateTracksCommand = new Command(updateTracksCommand);
        }

        private void updateTracksCommand()
        {
            TrackFileNames = _trackFileManager.GetTrackFilesFromDirectory().Select(t=>new TrackFileElement()
            {
                Filename = t.Name,
                CreateDate = t.CreationTime
            }).ToObservableCollection();
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
                    bool result = await tryParseAndGetTrackAsync(value.Filename, _routeId);
                    UserDialogs.Instance.Alert(title: "Парсинг и загрузка",
                        message: "Result:" + result, okText: "Ok");

                    value = null;
                });
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