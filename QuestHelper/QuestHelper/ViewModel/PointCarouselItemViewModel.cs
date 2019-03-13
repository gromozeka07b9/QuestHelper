using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class PointCarouselItemViewModel : INotifyPropertyChanged
    {
        private readonly ViewRoute _routeObject;
        private readonly ViewRoutePoint _pointObject;
        private readonly ViewRoutePointMediaObject _mediaObject;
        private Aspect _imageAspect;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        
        public ICommand ChangeImageAspectCommand { get; private set; }
        public ICommand EditRoutePointCommand { get; private set; }
        


        public PointCarouselItemViewModel(string routeId, string routePointId, string routePointMediaId)
        {
            _routeObject = new ViewRoute(routeId);

            _pointObject = new ViewRoutePoint(routeId, routePointId);

            _mediaObject = new ViewRoutePointMediaObject();
            _mediaObject.Load(routePointMediaId);

            ChangeImageAspectCommand = new Command(changeImageAspectCommand);
            EditRoutePointCommand = new Command(editRoutePointCommand);
            PhotoImageAspect = Aspect.AspectFill;

        }

        private void editRoutePointCommand(object obj)
        {
            var page = new RoutePointPage(_routeObject.RouteId, _pointObject.RoutePointId);
            Navigation.PushAsync(page);
        }

        private void changeImageAspectCommand(object obj)
        {
            if (PhotoImageAspect == Aspect.AspectFit)
            {
                PhotoImageAspect = Aspect.AspectFill;
            }
            else
            {
                PhotoImageAspect = Aspect.AspectFit;
            }
        }

        public Aspect PhotoImageAspect
        {
            get
            {
                return _imageAspect;
            }
            set
            {
                if (_imageAspect != value)
                {
                    _imageAspect = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PhotoImageAspect"));
                }
            }
        }
        public string RouteName
        {
            get { return _routeObject?.Name.ToUpper(); }
        }
        public string RoutePointName
        {
            get { return _pointObject?.Name.ToUpper(); }
        }
        public string RoutePointDescription
        {
            get
            {
                string text = string.Empty;
                if (string.IsNullOrEmpty(_pointObject?.Description))
                {
                    if (!string.IsNullOrEmpty(_pointObject.Address))
                    {
                        text = _pointObject.Address;
                    }
                }
                else text = _pointObject?.Description;
                return text;
            }
        }

        public bool FullscreenButtonIsVisible
        {
            get
            {
                return !string.IsNullOrEmpty(_mediaObject.RoutePointMediaObjectId);
            }
        }
        public string OneImagePreview
        {
            get
            {
                return ImagePathManager.GetImagePath(_mediaObject.RoutePointMediaObjectId, true);
            }
        }
        public string OneImage
        {
            get
            {
                return ImagePathManager.GetImagePath(_mediaObject.RoutePointMediaObjectId, false);
            }
        }
    }
}
