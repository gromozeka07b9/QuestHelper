using QuestHelper.Managers;
using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class PointCarouselItemViewModel : INotifyPropertyChanged
    {
        private ViewRoutePointMediaObject _mediaObject;
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public PointCarouselItemViewModel(string routePointId, string routePointMediaId)
        {
            //return _vpoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new ImagePreview() { Source = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, true), MediaId = x.RoutePointMediaObjectId }).ToList();
            _mediaObject = new ViewRoutePointMediaObject();
            _mediaObject.Load(routePointMediaId);
        }

        public string OneImage => ImagePathManager.GetImagePath(_mediaObject.RoutePointMediaObjectId, true);
    }
}
