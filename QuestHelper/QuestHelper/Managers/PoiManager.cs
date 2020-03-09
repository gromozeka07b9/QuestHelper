using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using Realms;
using Xamarin.Essentials;

namespace QuestHelper.Managers
{
    public class PoiManager : RealmInstanceMaker
    {
        public PoiManager()
        {
        }

        /*internal IEnumerable<ViewRoute> GetRoutes(string UserId)
        {
            List<ViewRoute> vroutes = new List<ViewRoute>();
            var routes = RealmInstance.All<Route>().Where(u=>(!u.IsDeleted && !u.IsPublished)||(!u.IsDeleted && u.IsPublished && u.CreatorId == UserId)).OrderByDescending(r => r.CreateDate);
            if (routes.Any())
            {
                foreach (var route in routes)
                {
                    vroutes.Add(new ViewRoute(route.RouteId));
                }
            }
            return vroutes;
        }

        internal int GetCountRoutesByCreator(string UserId)
        {
            List<ViewRoute> vroutes = new List<ViewRoute>();
            var countRoutes = RealmInstance.All<Route>().Where(u => (u.CreatorId == UserId)).Count();
            return countRoutes;
        }

        internal int GetCountPublishedRoutesByCreator(string UserId)
        {
            List<ViewRoute> vroutes = new List<ViewRoute>();
            var countRoutes = RealmInstance.All<Route>().Where(u => (u.CreatorId == UserId && u.IsPublished)).Count();
            return countRoutes;
        }

        internal IEnumerable<ViewRoute> GetRoutesForSync()
        {
            List<ViewRoute> vroutes = new List<ViewRoute>();
            var routes = RealmInstance.All<Route>().OrderByDescending(r => r.CreateDate);
            if (routes.Any())
            {
                foreach (var route in routes)
                {
                    vroutes.Add(new ViewRoute(route.RouteId));
                }
            }
            return vroutes;
        }
        internal List<ViewRoute> GetAllRoutes()
        {
            List<ViewRoute> vroutes = new List<ViewRoute>();
            var routes = RealmInstance.All<Route>().Where(r=>!r.IsDeleted).OrderByDescending(r => r.CreateDate);
            if (routes.Any())
            {
                foreach (var route in routes)
                {
                    vroutes.Add(new ViewRoute(route.RouteId));
                }
            }
            return vroutes;
        }

        /// <summary>
        /// На данном этапе публичные маршруты ничем почти не отличаются от обычных
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ViewRoute> GetPosts()
        {
            List<ViewRoute> vroutes = new List<ViewRoute>();
            var routes = RealmInstance.All<Route>().Where(r=>r.IsPublished && !r.IsDeleted).OrderByDescending(r => r.CreateDate);
            if (routes.Any())
            {
                foreach (var route in routes)
                {
                    vroutes.Add(new ViewRoute(route.RouteId));
                }
            }
            return vroutes;
        }

        /// <summary>
        /// Возвращает список маршрутов, созданных и опубликованных не текущим пользователем
        /// </summary>
        /// <param name="currentUserId"></param>
        /// <returns></returns>
        internal IEnumerable<ViewRoute> GetPostsOtherCreators(string currentUserId)
        {
            List<ViewRoute> vroutes = new List<ViewRoute>();
            var routes = RealmInstance.All<Route>().Where(r => r.IsPublished && !r.CreatorId.Equals(currentUserId)).OrderByDescending(r => r.CreateDate);
            if (routes.Any())
            {
                foreach (var route in routes)
                {
                    vroutes.Add(new ViewRoute(route.RouteId));
                }
            }
            return vroutes;
        }

        internal void DeleteRoutesDataFromStorage(IEnumerable<ViewRoute> routes)
        {
            RoutePointManager pointManager = new RoutePointManager();
            RoutePointMediaObjectManager mediaManager = new RoutePointMediaObjectManager();
            foreach (var vRoute in routes)
            {
                var points = pointManager.GetPointsByRouteId(vRoute.RouteId);
                foreach (var vPoint in points)
                {
                    var medias = mediaManager.GetMediaObjectsByRoutePointId(vPoint.RoutePointId);
                    foreach (var media in medias)
                    {
                        mediaManager.TryDeleteFile(media.RoutePointMediaObjectId, MediaObjectTypeEnum.Image, false);
                        mediaManager.TryDeleteFile(media.RoutePointMediaObjectId, MediaObjectTypeEnum.Image, true);
                        mediaManager.TryDeleteFile(media.RoutePointMediaObjectId, MediaObjectTypeEnum.Audio);
                        var vMedia = new ViewRoutePointMediaObject();
                        vMedia.Load(media.RoutePointMediaObjectId);
                        mediaManager.DeleteObjectFromLocalStorage(vMedia);
                    }
                    pointManager.DeleteObjectFromLocalStorage(vPoint);
                }
                DeleteObjectFromLocalStorage(vRoute);
            }
        }

        internal void DeleteObjectFromLocalStorage(ViewRoute vRoute)
        {
            if (vRoute != null)
            {
                try
                {
                    Route route = !string.IsNullOrEmpty(vRoute.Id) ? RealmInstance.Find<Route>(vRoute.Id) : null;
                    if (route != null)
                    {
                        RealmInstance.Write(() =>
                        {
                            RealmInstance.Remove(route);
                        });
                    }
                }
                catch (Exception e)
                {
                    HandleError.Process("RouteManager", "DeleteObjectFromLocalStorage", e, false);
                }
            }
        }

        public IEnumerable<Route> GetNotSynced()
        {
            return RealmInstance.All<Route>().Where(item => !item.ServerSynced);
        }*/

        internal void DeleteAll()
        {
            try
            {
                RealmInstance.Write(() =>
                {
                    RealmInstance.RemoveAll<Poi>();
                });
            }
            catch (Exception e)
            {
                HandleError.Process("PoiManager", "DeleteAll", e, false);
            }
        }

        public bool Save(ViewPoi viewPoi)
        {
            bool result = false;
            RouteManager routeManager = new RouteManager();
            try
            {
                RealmInstance.Write(() =>
                {
                    var poi = !string.IsNullOrEmpty(viewPoi.Id) ? RealmInstance.Find<Poi>(viewPoi.Id) : null;
                    if (null == poi)
                    {
                        poi = string.IsNullOrEmpty(viewPoi.Id) ? new Poi() : new Poi() { PoiId = viewPoi.Id };
                        RealmInstance.Add(poi);
                    }
                    poi.Name = viewPoi.Name;
                    poi.CreateDate = viewPoi.CreateDate;
                    poi.UpdateDate = viewPoi.UpdateDate;
                    poi.IsDeleted = viewPoi.IsDeleted;
                    poi.CreatorId = viewPoi.CreatorId;
                    poi.ImgFilename = viewPoi.ImgFilename;
                    poi.Description = viewPoi.Description;
                    //poi.PoiType = viewPoi.PoiType;
                    poi.Address = viewPoi.Address;
                    poi.ByRoutePointId = viewPoi.ByRoutePointId;
                    poi.IsPublished = viewPoi.IsPublished;
                    poi.Latitude = viewPoi.Location.Latitude;
                    poi.Longitude = viewPoi.Location.Longitude;
                    poi.LikesCount = viewPoi.LikesCount;
                    poi.ViewsCount = viewPoi.ViewsCount;
                });
                var poiSaved = RealmInstance.Find<Poi>(viewPoi.Id);
                viewPoi.Refresh(poiSaved.PoiId);
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("PoiManager", "SavePoi", e, false);
            }
            return result;
        }

        internal Poi GetPoiById(string id)
        {
            return RealmInstance.Find<Poi>(id);
        }

        internal ViewPoi GetViewPoiById(string id)
        {
            return new ViewPoi(id);
        }

    }
}
