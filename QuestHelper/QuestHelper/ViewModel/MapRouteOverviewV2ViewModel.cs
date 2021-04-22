using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QuestHelper.Annotations;
using QuestHelper.Managers;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class MapRouteOverviewV2ViewModel : INotifyPropertyChanged, IDialogEvents
    {
        private readonly string _routeId;
        private readonly TrackFileManager _trackFileManager;
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }

        public MapRouteOverviewV2ViewModel(string routeId)
        {
            _routeId = routeId;
            _trackFileManager = new TrackFileManager();
        }

        public IEnumerable<Tuple<double?, double?>> GetTrackPlaces()
        {
            return _trackFileManager.GetTrackByRoute(_routeId);
        } 
        public void StartDialog()
        {
        }

        public void CloseDialog()
        {

        }
    }
}