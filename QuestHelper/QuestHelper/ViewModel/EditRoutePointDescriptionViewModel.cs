using Plugin.Geolocator;
using QuestHelper.Managers;
using QuestHelper.LocalDB.Model;
using QuestHelper.View;
using QuestHelper.WS;
using Realms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using QuestHelper.Model;
using Microsoft.AppCenter.Analytics;

namespace QuestHelper.ViewModel
{
    public class EditRoutePointDescriptionViewModel : INotifyPropertyChanged
    {
        ViewRoutePoint _vpoint;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public EditRoutePointDescriptionViewModel(string routePointId)
        {
            if(!string.IsNullOrEmpty(routePointId))
            {
                RoutePointManager manager = new RoutePointManager();
                var point = manager.GetPointById(routePointId);
                _vpoint = new ViewRoutePoint(point.RouteId, routePointId);
            } else
            {
                HandleError.Process("EditRoutePointDescription", "EditDescripton", new Exception("Ошибка, точка еще не создана."), true);
            }
        }

        public void startDialog()
        {
        }

        public string Description
        {
            set
            {
                if (_vpoint.Description != value)
                {
                    _vpoint.Description = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                }
            }
            get
            {
                return _vpoint.Description;
            }
        }

        public void ApplyChanges()
        {
            Analytics.TrackEvent("Description edited");
            _vpoint.Version++;
            _vpoint.Save();
        }
    }
}
