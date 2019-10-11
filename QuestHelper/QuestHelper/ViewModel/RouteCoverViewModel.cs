using QuestHelper.LocalDB.Model;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace QuestHelper.ViewModel
{
    public class RouteCoverViewModel : INotifyPropertyChanged, IDialogEvents
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private RoutePointManager _routePointManager = new RoutePointManager();
        private ViewRoute _vroute;
        private readonly string _routeId;
        private ObservableCollection<ViewRoutePoint> _viewPointsOfRoute = new ObservableCollection<ViewRoutePoint>();

        public ICommand StartRouteCommand { get; private set; }

        public RouteCoverViewModel(string routeId)
        {
            StartRouteCommand = new Command(startRouteCommand);

            if (!string.IsNullOrEmpty(routeId))
            {
                _routeId = routeId;
                _vroute = new ViewRoute(routeId);
            }
            else throw new Exception("routeId is empty!");
            PointsOfRoute = new ObservableCollection<ViewRoutePoint>();
        }

        private void startRouteCommand(object obj)
        {
            var page = new RouteCarouselRootPage(_vroute.RouteId);
            Navigation.PushAsync(page);
        }

        public void CloseDialog()
        {
        }

        public void StartDialog()
        {
            var toolbarService = DependencyService.Get<IToolbarService>();
            toolbarService.SetVisibilityToolbar(false);

            var points = _routePointManager.GetPointsByRouteId(_vroute.Id);
            if (points.Any())
            {
                PointsOfRoute = new ObservableCollection<ViewRoutePoint>(points);
            }
        }

        public ObservableCollection<ViewRoutePoint> PointsOfRoute
        {
            set
            {
                if (_viewPointsOfRoute != value)
                {
                    _viewPointsOfRoute = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("PointsOfRoute"));
                    }
                }
            }
            get
            {
                return _viewPointsOfRoute;
            }
        }

        public string RouteCoverImage
        {
            get
            {
                return _vroute.ImagePreviewPathForList;
            }
        }
        public string Description
        {
            get { return "Длинное тестовое описание маршрута. Зачем его создали, кто создал, короткая справка о чем маршрут, а то может я и не захочу его загружать."; }
        }
        public string Name
        {
            get { return _vroute.Name; }
        }
        public string CreateDateText
        {
            get { return _vroute.CreateDate.ToString("yyyy MMMM"); }
        }
        public string Author
        {
            get { return "Sergey Dyachenko"; }
        }
    }
}
