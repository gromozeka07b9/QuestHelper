﻿using QuestHelper.Model;
using QuestHelper.WS;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using QuestHelper.SharedModelsWS;
using System.Collections.Generic;
using System.Text;

namespace QuestHelper.Managers.Sync
{
    public class SyncRoute : SyncRouteBase
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private string _authToken = string.Empty;
        private readonly string _routeId = string.Empty;
        private readonly RoutesApiRequest _routesApi;
        private readonly RoutePointsApiRequest _routePointsApi;
        private readonly RoutePointMediaObjectRequest _routePointMediaObjectsApi;
        private readonly RouteManager _routeManager = new RouteManager();
        private readonly RoutePointManager _routePointManager = new RoutePointManager();
        private readonly RoutePointMediaObjectManager _routePointMediaManager = new RoutePointMediaObjectManager();

        public SyncRoute(string routeId, string authToken)
        {
            _routeId = routeId;
            _authToken = authToken;
            _routesApi = new RoutesApiRequest(_apiUrl, _authToken);
            _routePointsApi = new RoutePointsApiRequest(_apiUrl, _authToken);
            _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl, _authToken);
        }

        public async Task<bool> SyncAsync(string routeServerHash)
        {
            var localRoute = _routeManager.GetViewRouteById(_routeId);
            if ((localRoute!=null) && (localRoute.ObjVerHash != routeServerHash))
            {
                if (string.IsNullOrEmpty(routeServerHash))
                {
                    //новый маршрут, отправить на сервер
                }
                else
                {
                    //есть изменения, какие - придется пройтись по всему маршруту и узнать
                    var routeRoot = await _routesApi.GetRouteRoot(_routeId);
                    bool AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                    if ((!AuthRequired) && (routeRoot != null))
                    {
                        bool updateResult = await updateRoute(routeServerHash, routeRoot, localRoute);
                        if (!updateResult) return false;

                        updateResult = await updatePoints(routeRoot);
                        if (!updateResult) return false;

                        updateResult = await updateMedias(routeRoot);
                        if (!updateResult) return false;

                    }
                    else return false;
                }
            }

            var updatedLocalRoute = _routeManager.GetViewRouteById(_routeId);
            StringBuilder sbVersions = getVersionsForRoute(updatedLocalRoute);
            refreshHashForRoute(updatedLocalRoute, sbVersions);

            return true;
        }

        private StringBuilder getVersionsForRoute(ViewRoute updatedLocalRoute)
        {
            StringBuilder versions = new StringBuilder();

            versions.Append(updatedLocalRoute.Version);

            var points = _routePointManager.GetPointsByRouteId(updatedLocalRoute.RouteId).OrderBy(p => p.RoutePointId);
            foreach (var point in points)
            {
                versions.Append(point.Version);
            }

            var medias = _routePointMediaManager.GetMediaObjectsByRouteId(updatedLocalRoute.RouteId).OrderBy(m => m.RoutePointMediaObjectId);
            foreach (var media in medias)
            {
                versions.Append(media.Version);
            }

            return versions;
        }

        private async Task<bool> updateMedias(RouteRoot routeRoot)
        {
            bool updateResult = true;

            List<string> mediasToUpload = new List<string>();

            var medias = _routePointMediaManager.GetMediaObjectsByRouteId(routeRoot.Route.Id);

            List<RoutePointMediaObject> serverMedias = new List<RoutePointMediaObject>();
            foreach (var serverPoint in routeRoot.Route.Points)
            {                
                serverMedias.AddRange(serverPoint.MediaObjects.Select(m => m));
            }

            var newMedias = medias.Where(m => !serverMedias.Any(sm => sm.Id == m.RoutePointMediaObjectId));
            //новые медиа
            mediasToUpload.AddRange(newMedias.Select(m=>m.RoutePointMediaObjectId));

            foreach (var serverMedia in serverMedias)
            {
                var localMedia = _routePointMediaManager.GetMediaObjectById(serverMedia.Id);
                if ((localMedia == null) || (serverMedia.Version > localMedia.Version))
                {
                    ViewRoutePointMediaObject updateMedia = new ViewRoutePointMediaObject();
                    updateMedia.FillFromWSModel(serverMedia);
                    updateResult = updateMedia.Save();
                }
                else if (serverMedia.Version < localMedia.Version)
                {
                    //в очередь на отправку
                    mediasToUpload.Add(serverMedia.Id);
                }
                if (!updateResult) return false;

            }

            if (mediasToUpload.Count > 0)
            {
                List<ViewRoutePointMediaObject> viewMediasToUpload = new List<ViewRoutePointMediaObject>();
                foreach (string mediaId in mediasToUpload)
                {
                    var media = new ViewRoutePointMediaObject();
                    media.Load(mediaId);
                    viewMediasToUpload.Add(media);
                }

                updateResult = await UploadAsync(GetJsonStructuresMedias(viewMediasToUpload), _routePointMediaObjectsApi);
            }

            return updateResult;
        }

        private async Task<bool> updatePoints(RouteRoot routeRoot)
        {
            bool updateResult = true;

            List<string> pointsToUpload = new List<string>();
            var pointsByRoute = _routePointManager.GetPointsByRouteId(_routeId);
            //если есть новые точки, на отправку
            pointsToUpload.AddRange(pointsByRoute
                .Where(p => !routeRoot.Route.Points.Any(sp => sp.Id == p.RoutePointId)).Select(p => p.Id)
                .ToList());

            foreach (var serverPoint in routeRoot.Route.Points)
            {
                var localPoint = _routePointManager.GetPointById(serverPoint.Id);
                if ((localPoint == null) || (serverPoint.Version > localPoint.Version))
                {
                    ViewRoutePoint updatePoint = new ViewRoutePoint(serverPoint.RouteId, serverPoint.Id);
                    updatePoint.FillFromWSModel(serverPoint);
                    updateResult = updatePoint.Save();
                }
                else if (serverPoint.Version < localPoint.Version)
                {
                    //на сервере более старая версия, в очередь на отправку
                    pointsToUpload.Add(serverPoint.Id);
                }

                if (!updateResult) return false;
            }


            if (pointsToUpload.Count > 0)
            {

                List<ViewRoutePoint> viewPointsToUpload = new List<ViewRoutePoint>();

                foreach (string routePointId in pointsToUpload)
                {
                    viewPointsToUpload.Add(new ViewRoutePoint(routeRoot.Route.Id, routePointId));
                }

                updateResult = await UploadAsync(GetJsonStructuresPoints(viewPointsToUpload), _routePointsApi);
            }

            return updateResult;
        }

        private async Task<bool> updateRoute(string routeServerHash, RouteRoot routeRoot, ViewRoute localRoute)
        {
            bool updateResult = true;

            if ((localRoute == null) || (routeRoot.Route.Version > localRoute.Version))
            {
                ViewRoute updateViewRoute = new ViewRoute(_routeId);
                updateViewRoute.FillFromWSModel(routeRoot, routeServerHash);
                updateResult = updateViewRoute.Save();
            }
            else if (routeRoot.Route.Version < localRoute.Version)
            {
                updateResult = await UploadAsync(GetRouteJsonStructure(localRoute), _routesApi);
            }

            return updateResult;
        }
    }
}
