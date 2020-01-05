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
using QuestHelper.Model.Messages;

namespace QuestHelper.ViewModel
{
    public class EditRoutePointDescriptionViewModel : INotifyPropertyChanged
    {
        ViewRoutePoint _vpoint;
        private bool _isEditMode = false;
        private string _previousDescription = string.Empty;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand BackNavigationCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand EditDescriptionCommand { get; private set; }

        public EditRoutePointDescriptionViewModel(string routePointId)
        {
            BackNavigationCommand = new Command(backNavigationCommand);
            CancelCommand = new Command(cancelCommand);
            EditDescriptionCommand = new Command(editDescriptionCommand);
            if (!string.IsNullOrEmpty(routePointId))
            {
                RoutePointManager manager = new RoutePointManager();
                var point = manager.GetPointById(routePointId);
                _vpoint = new ViewRoutePoint(point.RouteId, routePointId);
                _previousDescription = _vpoint.Description;
            } else
            {
                HandleError.Process("EditRoutePointDescription", "EditDescripton", new Exception("Ошибка, точка еще не создана."), true);
            }

            IsEditMode = false;
        }

        private void cancelCommand(object obj)
        {
            Description = _previousDescription;
        }

        private void backNavigationCommand(object obj)
        {
            SaveChangedText();
            Navigation.PopModalAsync();
        }

        private void editDescriptionCommand(object obj)
        {
            IsEditMode = true;
        }

        public void startDialog()
        {
        }

        public string Description
        {
            set
            {
                IsTextModified = (_vpoint.Description != value);
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

        public void SaveChangedText()
        {
            if (IsTextModified)
            {
                ApplyChanges();
                MessagingCenter.Send<RoutePointDescriptionModifiedMessage>(new RoutePointDescriptionModifiedMessage() { RoutePointId = _vpoint.Id }, string.Empty);
            }
        }

        public void ApplyChanges()
        {
            Analytics.TrackEvent("Description edited");
            _vpoint.Version++;
            _vpoint.Save();
        }

        public bool IsEditMode
        {
            set
            {
                if (_isEditMode != value)
                {
                    _isEditMode = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsEditMode"));
                    }
                }
            }
            get
            {
                return _isEditMode;
            }
        }
        public bool IsTextModified { get; private set; }

    }
}
