using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using QuestHelper.Model;
using Xamarin.Forms;
using QuestHelper.Managers;
using System.Collections.ObjectModel;
using QuestHelper.View;
using Acr.UserDialogs;

namespace QuestHelper.ViewModel
{
    public class RoutePointCarouselViewModel
    {
        public INavigation Navigation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private string _routeId = string.Empty;

        private RouteManager _routeManager = new RouteManager();
        private RoutePointManager _routePointManager = new RoutePointManager();

        public RoutePointCarouselViewModel(string routeId)
        {
            _routeId = routeId;
        }
        internal void CloseDialog()
        {
        }

        public void StartDialog()
        {
        }

        public List<ContentPage> CarouselPages
        {
            get
            {
                //DateTime startTime = DateTime.Now;
                List<ContentPage> pages = new List<ContentPage>();
                var points = _routePointManager.GetPointsByRouteId(_routeId);
                if (points.Any())
                {
                    foreach (var point in points)
                    {
                        if (point.MediaObjects.Any())
                        {
                            foreach (var media in point.MediaObjects)
                            {
                                pages.Add(new PointCarouselItemPage(_routeId, point.RoutePointId, media.RoutePointMediaObjectId));
                            }
                        }
                        else
                        {
                            pages.Add(new PointCarouselItemPage(_routeId, point.RoutePointId, string.Empty));
                        }
                    }
                }
                //var diff = DateTime.Now - startTime;
                //DependencyService.Get<IToastService>().LongToast("delay:" + diff.TotalMilliseconds.ToString());
                return pages;
            }
        }
    }
}