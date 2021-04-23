using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QuestHelper.Managers;
using QuestHelper.Model;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class MapRouteOverviewV2ViewModel : INotifyPropertyChanged, IDialogEvents
    {
        private readonly string _routeId;
        private readonly TrackFileManager _trackFileManager;
        private readonly RoutePointManager _routePointManager;
        public event PropertyChangedEventHandler PropertyChanged;
        public INavigation Navigation { get; set; }

        public MapRouteOverviewV2ViewModel(string routeId)
        {
            _routeId = routeId;
            _trackFileManager = new TrackFileManager();
            _routePointManager = new RoutePointManager();
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
    }
}