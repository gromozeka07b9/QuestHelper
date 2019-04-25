using QuestHelper.LocalDB.Model;
using QuestHelper.WS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using QuestHelper.Model;
using Xamarin.Forms;
using Route = QuestHelper.SharedModelsWS.Route;

namespace QuestHelper.Managers.Sync
{
    public class SyncRoutes : SyncBase
    {
        private const string _apiUrl = "http://igosh.pro/api";
        private readonly RoutesApiRequest _routesApi;
        private readonly RoutePointsApiRequest _routePointsApi;
        private readonly RoutePointMediaObjectRequest _routePointMediaObjectsApi;
        private readonly RouteManager _routeManager = new RouteManager();
        private readonly RoutePointManager _routePointManager = new RoutePointManager();
        private readonly RoutePointMediaObjectManager _routePointMediaManager = new RoutePointMediaObjectManager();
        private readonly string _authToken = string.Empty;

        public SyncRoutes(string authToken)
        {
            _authToken = authToken;
            _routesApi = new RoutesApiRequest(_apiUrl, _authToken);
            _routePointsApi = new RoutePointsApiRequest(_apiUrl, _authToken);
            _routePointMediaObjectsApi = new RoutePointMediaObjectRequest(_apiUrl, _authToken);
        }

        public async Task<bool> Sync()
        {
            bool result = false;
            var listRoutesVersions = await _routesApi.GetRoutesVersions();
            AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
            if ((!AuthRequired) && (listRoutesVersions != null))
            {
                var routesLocal = _routeManager.GetRoutesForSync().Select(x => new {x.RouteId, x.Version, x.ObjVerHash});
                var differentRoutes = listRoutesVersions.Where(r=>(!routesLocal.Any(l=>(l.RouteId == r.Id && l.ObjVerHash == r.ObjVerHash))));
                List<Tuple<string, string>> newServerRoutes = new List<Tuple<string, string>>();
                List<string> newClientRoutes = new List<string>();
                List<Tuple<string, string>> changedRoutes = new List<Tuple<string, string>>();
                foreach (var routeVersion in differentRoutes)
                {
                    var localRoute = routesLocal.Where(r => r.RouteId == routeVersion.Id).SingleOrDefault();
                    if (localRoute != null)
                    {
                        changedRoutes.Add(new Tuple<string, string>(routeVersion.Id, routeVersion.ObjVerHash));
                    }
                    else
                    {
                        //новый маршрут на сервере, грузить
                        newServerRoutes.Add(new Tuple<string,string>(routeVersion.Id, routeVersion.ObjVerHash));
                    }
                }

                newClientRoutes = routesLocal.Where(r => !listRoutesVersions.Any(d => d.Id == r.RouteId)).Select(r=>r.RouteId).ToList();

                bool resultUpdate = await UpdateRoutesAsync(changedRoutes);
                bool resultUpload = await UploadRoutes(newClientRoutes);
                bool resultDownload = await DownloadRoutesAsync(newServerRoutes);
                if ((changedRoutes.Count > 0) || (newClientRoutes.Count > 0) || (newServerRoutes.Count > 0))
                {
                    List<string> routesForGenerateHash = new List<string>();
                    routesForGenerateHash.AddRange(changedRoutes.Select(r=>r.Item1));
                    routesForGenerateHash.AddRange(newClientRoutes.Select(r=>r));
                    routesForGenerateHash.AddRange(newServerRoutes.Select(r => r.Item1));
                    generateHashForRoutes(routesForGenerateHash);
                }
            }
            return result;
        }

        private void generateHashForRoutes(List<string> routesForGenerateHash)
        {
            var routes = _routeManager.GetRoutes(routesForGenerateHash);
            foreach (var route in routes)
            {
                StringBuilder versions = new StringBuilder();
                versions.Append(route.Version);
                var points = _routePointManager.GetPointsByRouteId(route.RouteId).OrderBy(p => p.RoutePointId);
                foreach (var point in points)
                {
                    versions.Append(point.Version);
                }

                var medias = _routePointMediaManager.GetMediaObjectsByRouteId(route.RouteId).OrderBy(m => m.RoutePointMediaObjectId);
                foreach (var media in medias)
                {
                    versions.Append(media.Version);
                }
                string test = HashManager.Generate("322");
                route.ObjVerHash = HashManager.Generate(versions.ToString());
                route.Save();
            }
        }

        private async Task<bool> UpdateRoutesAsync(List<Tuple<string, string>> changedRoutes)
        {
            List<string> routesToUpload = new List<string>();
            List<string> pointsToUpload = new List<string>();
            List<string> mediasToUpload = new List<string>();

            foreach (var updateRoute in changedRoutes)
            {
                var routeRoot = await _routesApi.GetRouteRoot(updateRoute.Item1);
                AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                if ((!AuthRequired) && (routeRoot != null))
                {
                    bool updateResult = false;
                    var localRoute = _routeManager.GetRouteById(routeRoot.Route.Id);
                    if ((localRoute == null) || (routeRoot.Route.Version > localRoute.Version))
                    {
                        ViewRoute updateViewRoute = new ViewRoute(updateRoute.Item1);
                        updateViewRoute.FillFromWSModel(routeRoot, updateRoute.Item2);
                        updateResult = updateViewRoute.Save();

                    }
                    else if(routeRoot.Route.Version < localRoute.Version)
                    {
                        //в очередь на отправку
                        routesToUpload.Add(routeRoot.Route.Id);
                    }
                    else if (routeRoot.Route.Version == localRoute.Version)
                    {
                        //нет изменений, они где-то глубже чем на уровне точки/медиа
                        updateResult = true;
                    }
                    if (updateResult)
                    {
                        var pointsByRoute = _routePointManager.GetPointsByRouteId(updateRoute.Item1);
                        var newClientPoints = pointsByRoute
                            .Where(p => !routeRoot.Route.Points.Any(sp => sp.Id == p.RoutePointId)).Select(p => p.Id)
                            .ToList();
                        pointsToUpload.AddRange(newClientPoints);

                        //var mediasByRoute = _routePointMediaManager.GetMediaObjectsByRouteId(updateRoute.Item1);
                        //var newMedias = mediasByRoute.Where(m => !routeRoot.Route.Points.Any(sp => sp.Id == p.RoutePointId))

                        foreach (var serverPoint in routeRoot.Route.Points)
                        {
                            var localPoint = _routePointManager.GetPointById(serverPoint.Id);
                            if((localPoint == null) || (serverPoint.Version > localPoint.Version))
                            {
                                ViewRoutePoint updatePoint = new ViewRoutePoint(serverPoint.RouteId, serverPoint.Id);
                                updatePoint.FillFromWSModel(serverPoint);
                                updateResult = updatePoint.Save();
                            }
                            else if (serverPoint.Version < localPoint.Version)
                            {
                                //в очередь на отправку
                                pointsToUpload.Add(serverPoint.Id);
                                updateResult = true;
                            }
                            else if (serverPoint.Version == localPoint.Version)
                            {
                                //нет изменений, они где-то глубже чем на уровне медиа
                                updateResult = true;
                            }
                            if (updateResult)
                            {
                                foreach (var mediaObject in serverPoint.MediaObjects)
                                {
                                    var localMedia = _routePointMediaManager.GetMediaObjectById(mediaObject.Id);
                                    if ((localMedia == null) || (mediaObject.Version > localMedia.Version))
                                    {
                                        ViewRoutePointMediaObject updateMedia = new ViewRoutePointMediaObject();
                                        updateMedia.FillFromWSModel(mediaObject);
                                        updateResult = updateMedia.Save();
                                    }
                                    else if (mediaObject.Version < localMedia.Version)
                                    {
                                        //в очередь на отправку
                                        mediasToUpload.Add(mediaObject.Id);
                                        updateResult = true;
                                    }
                                    else if (mediaObject.Version == localMedia.Version)
                                    {
                                        //нет изменений, скорее всего какой-то косяк
                                        updateResult = true;
                                    }

                                    if (!updateResult) return false;
                                }
                            }
                            else return false;
                        }                        
                    }
                    else return false;

                    if (updateResult)
                    {
                        if (routesToUpload.Count > 0)
                        {
                            await UploadAsync(GetJsonStructures(routesToUpload), _routesApi);
                        }
                        if (pointsToUpload.Count > 0)
                        {
                            await UploadAsync(GetJsonStructuresPoints(pointsToUpload), _routePointsApi);
                        }
                        if (mediasToUpload.Count > 0)
                        {
                            await UploadAsync(GetJsonStructuresMedias(mediasToUpload), _routePointMediaObjectsApi);
                        }
                    }
                }
                else if (AuthRequired) return false;
            }
            return true;
        }

        private async Task<bool> UploadRoutes(List<string> newClientRoutes)
        {
            return await UploadAsync(GetJsonStructures(newClientRoutes), _routesApi);
        }

        private async Task<bool> DownloadRoutesAsync(List<Tuple<string, string>> newServerRoutes)
        {
            foreach (var serverRoute in newServerRoutes)
            {
                var routeRoot = await _routesApi.GetRouteRoot(serverRoute.Item1);
                AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                if ((!AuthRequired) && (routeRoot != null))
                {
                    ViewRoute localRoute = new ViewRoute(serverRoute.Item1);
                    localRoute.FillFromWSModel(routeRoot, serverRoute.Item2);
                    bool saveResult = localRoute.Save();
                    if (saveResult)
                    {
                        foreach (var serverPoint in routeRoot.Route.Points)
                        {
                            ViewRoutePoint localPoint = new ViewRoutePoint(serverPoint.RouteId, serverPoint.Id);
                            localPoint.FillFromWSModel(serverPoint);
                            saveResult = localPoint.Save();
                            if (saveResult)
                            {
                                foreach (var mediaObject in serverPoint.MediaObjects)
                                {
                                    ViewRoutePointMediaObject localMedia = new ViewRoutePointMediaObject();
                                    localMedia.FillFromWSModel(mediaObject);
                                    saveResult = localMedia.Save();
                                    if (!saveResult) return false;
                                }
                            }
                            else return false;
                        }
                    }
                    else return false;
                } else if (AuthRequired) return false;
            }
            return true;
        }

        /*public async Task<bool> Sync()
{
   bool result = false;

   var routes = _routeManager.GetRoutesForSync().Select(x => new Tuple<string, int>(x.RouteId, x.Version));
   SyncObjectStatus routeServerStatus = await _routesApi.GetSyncStatus(routes);
   AuthRequired = (_routesApi.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || _routesApi.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
   if (routeServerStatus != null)
   {
       result = true;
       List<string> routesForUpload = new List<string>();
       List<string> routesForDownload = new List<string>();

       FillListsObjectsForProcess(routes, routeServerStatus, routesForUpload, routesForDownload);
       if (routesForUpload.Count > 0)
       {
           result = await UploadAsync(GetJsonStructures(routesForUpload), _routesApi);
           if (!result) return result;
       }

       if (routesForDownload.Count > 0)
       {
           result = await DownloadAsync(routesForDownload, _routesApi);
       }
   }

   return result;
}*/

        private List<string> GetJsonStructures(List<string> routesForUpload)
        {            
            List<string> jsonStructures = new List<string>();
            foreach (var routeId in routesForUpload)
            {
                var uploadedObject = _routeManager.GetRouteById(routeId);
                if (uploadedObject != null)
                {
                    JObject jsonObject = JObject.FromObject(new
                    {
                        RouteId = uploadedObject.RouteId,
                        Name = uploadedObject.Name,
                        CreateDate = uploadedObject.CreateDate.DateTime,
                        Version = uploadedObject.Version,
                        IsPublished = uploadedObject.IsPublished,
                        CreatorId = uploadedObject.CreatorId,
                        UserId = 0
                    });
                    jsonStructures.Add(jsonObject.ToString());
                }
            }

            return jsonStructures;
        }

        internal List<string> GetJsonStructuresPoints(List<string> pointsForUpload)
        {
            List<string> jsonStructures = new List<string>();

            foreach (var pointId in pointsForUpload)
            {
                var uploadedObject = _routePointManager.GetPointById(pointId);
                if ((uploadedObject != null) && (uploadedObject.CreateDate.Year > 2000))
                {
                    JObject jsonObject = JObject.FromObject(new
                    {
                        uploadedObject.RoutePointId,
                        uploadedObject.MainRoute.RouteId,
                        uploadedObject.Name,
                        CreateDate = uploadedObject.CreateDate.DateTime,
                        UpdateDate = uploadedObject.CreateDate.DateTime,
                        UpdatedUserId = "",
                        uploadedObject.Latitude,
                        uploadedObject.Longitude,
                        uploadedObject.Address,
                        uploadedObject.Description,
                        uploadedObject.Version
                    });
                    jsonStructures.Add(jsonObject.ToString());
                }
            }

            return jsonStructures;
        }
        private List<string> GetJsonStructuresMedias(List<string> mediasForUpload)
        {
            List<string> jsonStructures = new List<string>();

            foreach (var mediaId in mediasForUpload)
            {
                var uploadedObject = _routePointMediaManager.GetMediaObjectById(mediaId);
                if (uploadedObject != null)
                {
                    JObject jsonObject = JObject.FromObject(new
                    {
                        uploadedObject.RoutePointMediaObjectId,
                        uploadedObject.RoutePointId,
                        uploadedObject.Version,
                        uploadedObject.IsDeleted
                    });
                    jsonStructures.Add(jsonObject.ToString());
                }
            }

            return jsonStructures;
        }


    }
}
