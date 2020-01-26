using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class RouteGalleryViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private RoutePointManager _routePointManager = new RoutePointManager();
        private ViewRoute _vroute;
        private ViewRoutePoint _vCurrentPoint;
        private ViewRoutePoint _vNextPoint;
        private ViewRoutePoint _vPreviousPoint;
        private RoutePointItem _currentPointItem;
        private RoutePointItem _previousPointItem;
        private RoutePointItem _nextPointItem;
        private int _currentPointIndex = 1;
        private IEnumerable<RoutePointItem> _routePoints;
        public ICommand PositionItemChange { get; private set; }

        public RouteGalleryViewModel(string routeId)
        {
            PositionItemChange = new Command(positionItemChange);
            _vroute = new ViewRoute(routeId);
        }

        private void positionItemChange(object objIndex)
        {
            int imgCount = ImagesCurrentPoint.Count();
            int newPosition = (int)objIndex;
            if(newPosition == 0)
            {
                _currentPointIndex--;
                InitPointCollection();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagesCurrentPoint"));
            }
            else if(newPosition == imgCount - 1)
            {
                _currentPointIndex++;
                InitPointCollection();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagesCurrentPoint"));
            }
        }

        public class RoutePointItem
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }

        public IEnumerable<PointImageItem> ImagesCurrentPoint
        {
            get
            {
                if(_vCurrentPoint == null)
                {
                    InitPointCollection();
                }
                List<PointImageItem> list = new List<PointImageItem>();
                var firstItem = _vPreviousPoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new PointImageItem() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType, RoutePointId = x.RoutePointId }).FirstOrDefault();
                list.Add(firstItem);

                list.AddRange(_vCurrentPoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new PointImageItem() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType, RoutePointId = x.RoutePointId }).ToList());
                
                var nextItem = _vNextPoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new PointImageItem() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType, RoutePointId = x.RoutePointId }).LastOrDefault();
                list.Add(nextItem);
                return list;
            }
        }

        public class PointImageItem
        {
            public string SourceImg { get; set; }
            public string MediaId { get; set; }
            public string RoutePointId { get; set; }
            public MediaObjectTypeEnum MediaType { get; set; }
        }

        public void StartDialog()
        {
        }

        private void InitPointCollection()
        {
            var points = _routePointManager.GetPointsByRouteId(_vroute.RouteId, false);
            _routePoints = points.Where(x => !x.IsDeleted).OrderBy(x => x.CreateDate).Select(x => new RoutePointItem() { Name = x.Name, Id = x.Id }).ToList();
            if (_routePoints.Any())
            {
                _currentPointItem = _routePoints.ElementAt(_currentPointIndex);
                _nextPointItem = _currentPointIndex >= _routePoints.Count() - 1 ? new RoutePointItem() : _routePoints.ElementAt(_currentPointIndex + 1);
                _previousPointItem = _currentPointIndex <= 0 ? new RoutePointItem() : _routePoints.ElementAt(_currentPointIndex - 1);
                _vCurrentPoint = new ViewRoutePoint(_vroute.Id, _currentPointItem?.Id);
                _vNextPoint = new ViewRoutePoint(_vroute.Id, _nextPointItem?.Id);
                _vPreviousPoint = new ViewRoutePoint(_vroute.Id, _previousPointItem?.Id);
            }
        }

        public void CloseDialog()
        {
        }
    }
}
