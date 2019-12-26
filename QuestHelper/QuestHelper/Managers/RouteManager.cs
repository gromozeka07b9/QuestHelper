﻿using System;
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
    public class RouteManager : RealmInstanceMaker
    {
        public RouteManager()
        {
        }

        internal IEnumerable<ViewRoute> GetRoutes(string UserId)
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
        }

        public bool Save(ViewRoute viewRoute)
        {
            bool result = false;
            RouteManager routeManager = new RouteManager();
            try
            {
                RealmInstance.Write(() =>
                {
                    var route = !string.IsNullOrEmpty(viewRoute.Id) ? RealmInstance.Find<Route>(viewRoute.Id) : null;
                    if (null == route)
                    {
                        route = string.IsNullOrEmpty(viewRoute.Id) ? new Route() : new Route() { RouteId = viewRoute.Id };
                        RealmInstance.Add(route);
                    }
                    route.Name = viewRoute.Name;
                    route.Version = viewRoute.Version;
                    route.CreateDate = viewRoute.CreateDate;
                    route.IsShared = viewRoute.IsShared;
                    route.IsPublished = viewRoute.IsPublished;
                    route.IsDeleted = viewRoute.IsDeleted;
                    route.CreatorId = viewRoute.CreatorId;
                    route.ObjVerHash = viewRoute.ObjVerHash;
                    route.ImgFilename = viewRoute.ImgFilename;
                    route.Description = viewRoute.Description;
                    viewRoute.Refresh(route.RouteId);
                });
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RouteManager", "AddRoute", e, false);
            }
            return result;
        }

        internal Route GetRouteById(string routeId)
        {
            return RealmInstance.Find<Route>(routeId);
        }
        internal ViewRoute GetViewRouteById(string routeId)
        {
            return new ViewRoute(routeId);
        }

        internal (int pointCountInRoute, double length) GetLengthRouteData(string routeId)
        {
            double length = 0;
            int countPoints = 0;
            if (!string.IsNullOrEmpty(routeId))
            {
                var route = RealmInstance.Find<Route>(routeId);
                if (route != null)
                {
                    countPoints = route.Points.Count;
                    for (int index = 0; index < countPoints; index++)
                    {
                        if (index + 1 < countPoints)
                        {
                            var firstPoint = route.Points[index];
                            var secondPoint = route.Points[index + 1];
                            if((firstPoint.Latitude != 0)&&(firstPoint.Longitude!=0))
                            {
                                if ((secondPoint.Latitude != 0) && (secondPoint.Longitude != 0))
                                {
                                    Location firstPointLocation = new Location(firstPoint.Latitude, firstPoint.Longitude);
                                    Location secondPointLocation = new Location(secondPoint.Latitude, secondPoint.Longitude);
                                    length += Location.CalculateDistance(firstPointLocation, secondPointLocation, DistanceUnits.Kilometers);
                                }

                            }
                        }
                    }
                }
            }

            return (countPoints, length);
        }
    }
}
