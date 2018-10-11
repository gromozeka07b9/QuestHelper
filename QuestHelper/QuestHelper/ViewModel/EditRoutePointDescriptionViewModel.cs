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

namespace QuestHelper.ViewModel
{
    public class EditRoutePointDescriptionViewModel : INotifyPropertyChanged
    {
        private RoutePoint _routePoint;

        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public EditRoutePointDescriptionViewModel(RoutePoint routePoint)
        {
            _routePoint = routePoint;
        }

        public void startDialog()
        {
        }

        public string Description
        {
            set
            {
                if (_routePoint.Description != value)
                {
                    var realm = RoutePointManager.GetRealmInstance();
                    realm.Write(() =>
                    {
                        _routePoint.Description = value;
                    });
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description"));
                }
            }
            get
            {
                return _routePoint.Description;
            }
        }
    }
}
