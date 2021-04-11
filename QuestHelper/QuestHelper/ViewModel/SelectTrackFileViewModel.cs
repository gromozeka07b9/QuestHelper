using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using QuestHelper.Managers;
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
                TokenStoreService tokenService = new TokenStoreService();
                string _currentUserToken = tokenService.GetAuthTokenAsync().Result;
                TrackRouteRequest sendTrackRequest = new TrackRouteRequest(_currentUserToken);
                sendTrackRequest.SendTrackFileAsync(value.Filename, _routeId);
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