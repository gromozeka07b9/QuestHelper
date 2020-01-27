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
        private int _currentPointIndex = 0;
        private int _carouselCurrentItemPosition = 0;
        private IEnumerable<RoutePointItem> _routePoints;
        public ICommand PositionItemChange { get; private set; }
        public ICommand PrevPointCommand { get; private set; }
        public ICommand NextPointCommand { get; private set; }

        public RouteGalleryViewModel(string routeId)
        {
            PositionItemChange = new Command(positionItemChange);
            PrevPointCommand = new Command(prevPointCommand);
            NextPointCommand = new Command(nextPointCommand);
            _vroute = new ViewRoute(routeId);
        }

        private void nextPointCommand()
        {
            _currentPointIndex++;
            UpdateItemsByPosition(_currentPointIndex);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagesCurrentPoint"));
            Position = 0;
        }

        private void prevPointCommand()
        {
            _currentPointIndex--;
            UpdateItemsByPosition(_currentPointIndex);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagesCurrentPoint"));
            Position = 0;
        }

        private void positionItemChange(object objIndex)
        {
            /*int imgCount = ImagesCurrentPoint.Count();
            int newPosition = (int)objIndex;
            var currentItem = ImagesCurrentPoint.ElementAtOrDefault(newPosition);
            _currentPointIndex = currentItem.CurrentPointIndex;

            if (newPosition == 0)
            {
                UpdateItemsByPosition(_currentPointIndex);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagesCurrentPoint"));
            }
            else if(newPosition == imgCount - 1)
            {
                UpdateItemsByPosition(_currentPointIndex);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ImagesCurrentPoint"));
            }*/
        }

        public class RoutePointItem
        {
            public string Name { get; set; }
            public string Id { get; set; }
        }

        public int Position
        {
            set
            {
                if(value != _carouselCurrentItemPosition)
                {
                    _carouselCurrentItemPosition = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Position"));
                }
            }
            get
            {
                return _carouselCurrentItemPosition;
            }
        }
        public IEnumerable<PointImageItem> ImagesCurrentPoint
        {
            get
            {
                if(_vCurrentPoint == null)
                {
                    InitPointCollection();
                    UpdateItemsByPosition(_currentPointIndex);
                }
                List<PointImageItem> list = new List<PointImageItem>();
                string prevImgSrc = string.Empty;
                string nextImgSrc = string.Empty;
                if (!string.IsNullOrEmpty(_vPreviousPoint.Id))
                {
                    var firstItem = _vPreviousPoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new PointImageItem() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType, RoutePointId = x.RoutePointId, RoutePointName = _vPreviousPoint.Name, CurrentPointIndex = _currentPointIndex - 1 }).FirstOrDefault();
                    prevImgSrc = firstItem.SourceImg;
                    //firstItem.SourceImg = "baseline_clear_black_48.png";
                    //list.Add(firstItem);
                }

                if (!string.IsNullOrEmpty(_vNextPoint.Id))
                {
                    var nextItem = _vNextPoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new PointImageItem() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType, RoutePointId = x.RoutePointId, RoutePointName = _vNextPoint.Name, CurrentPointIndex = _currentPointIndex + 1 }).LastOrDefault();
                    nextImgSrc = nextItem.SourceImg;
                    //nextItem.SourceImg = "baseline_clear_black_48.png";
                    //list.Add(nextItem);
                }

                list.AddRange(_vCurrentPoint.MediaObjects.Where(x => !x.IsDeleted).Select(x => new PointImageItem() { SourceImg = ImagePathManager.GetImagePath(x.RoutePointMediaObjectId, (MediaObjectTypeEnum)x.MediaType, true), MediaId = x.RoutePointMediaObjectId, MediaType = (MediaObjectTypeEnum)x.MediaType, RoutePointId = x.RoutePointId, RoutePointName = _vCurrentPoint.Name, CurrentPointIndex = _currentPointIndex, PrevSourceImg = prevImgSrc, NextSourceImg = nextImgSrc }).ToList());
                return list;
            }
        }

        public class PointImageItem
        {
            public string SourceImg { get; set; }
            public string PrevSourceImg { get; set; }
            public string NextSourceImg { get; set; }
            public string MediaId { get; set; }
            public string RoutePointId { get; set; }
            public int CurrentPointIndex { get; set; }
            public MediaObjectTypeEnum MediaType { get; set; }
            public string RoutePointName { get; set; }
        }

        public void StartDialog()
        {
        }

        private void InitPointCollection()
        {
            var points = _routePointManager.GetPointsByRouteId(_vroute.RouteId, false);
            _routePoints = points.Where(x => !x.IsDeleted).OrderBy(x => x.CreateDate).Select(x => new RoutePointItem() { Name = x.Name, Id = x.Id }).ToList();
        }

        private void UpdateItemsByPosition(int index)
        {
            if (_routePoints != null && _routePoints.Any())
            {
                _currentPointItem = _routePoints.ElementAt(index);
                _nextPointItem = _currentPointIndex >= _routePoints.Count() - 1 ? new RoutePointItem() : _routePoints.ElementAt(index + 1);
                _previousPointItem = _currentPointIndex <= 0 ? new RoutePointItem() : _routePoints.ElementAt(index - 1);
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
