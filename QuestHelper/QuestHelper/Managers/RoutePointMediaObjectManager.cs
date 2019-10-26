using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using Realms;

namespace QuestHelper.Managers
{
    public class RoutePointMediaObjectManager : RealmInstanceMaker
    {
        public RoutePointMediaObjectManager()
        {
        }

        public bool SetSyncStatus(string Id, bool IsSyncPreview, bool Status)
        {
            bool result = false;
            try
            {
                RealmInstance.Write(() =>
                {
                    var mediaObject = RealmInstance.Find<RoutePointMediaObject>(Id);
                    if(IsSyncPreview)
                        mediaObject.PreviewServerSynced = Status;
                    else
                        mediaObject.OriginalServerSynced = Status;
                }
                );
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectManager", "SetSyncStatus", e, false);
            }
            return result;
        }

        internal bool Delete(string mediaObjectId)
        {
            bool result = false;
            try
            {
                RealmInstance.Write(() =>
                    {
                        var mediaObject = RealmInstance.Find<RoutePointMediaObject>(mediaObjectId);
                        if (mediaObject != null)
                        {
                            var pointObject = RealmInstance.Find<RoutePoint>(mediaObject.RoutePointId);
                            if (pointObject != null)
                            {
                                mediaObject.IsDeleted = true;
                                mediaObject.Version++;
                                result = mediaObject.IsDeleted;
                                //нужно для того, чтобы синхронизация обнаружила отличия от сервера и проверила версии с последующей отправкой изменений на сервер
                                pointObject.MainRoute.ObjVerHash = string.Empty;
                            }
                        }
                    }
                );
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectManager", "Delete", e, false);
            }
            return result;
        }

        internal string Save(ViewRoutePointMediaObject vmedia)
        {
            string returnId = string.Empty;

            try
            {
                var pointObject = RealmInstance.Find<RoutePoint>(vmedia.RoutePointId);
                RealmInstance.Write(() =>
                {
                    RoutePointMediaObject mediaObject = !string.IsNullOrEmpty(vmedia.Id) ? RealmInstance.Find<RoutePointMediaObject>(vmedia.Id) : null;
                    if (mediaObject == null)
                    {
                        mediaObject = new RoutePointMediaObject();
                        mediaObject.RoutePointMediaObjectId = vmedia.Id;
                        mediaObject.RoutePointId = vmedia.RoutePointId;
                        mediaObject.Point = pointObject;
                        //mediaObject.IsDeleted = vmedia.IsDeleted;
                        //mediaObject.MediaType = (int)vmedia.MediaType;
                        RealmInstance.Add(mediaObject);
                    }

                    if (mediaObject.Version != vmedia.Version)
                    {
                        //нужно для того, чтобы синхронизация обнаружила отличия от сервера и проверила версии с последующей отправкой изменений на сервер
                        var route = pointObject.MainRoute;
                        route.ObjVerHash = string.Empty;
                    }

                    returnId = mediaObject.RoutePointMediaObjectId;
                    mediaObject.OriginalServerSynced = vmedia.OriginalServerSynced;
                    mediaObject.PreviewServerSynced = vmedia.PreviewServerSynced;
                    mediaObject.ServerSyncedDate = vmedia.ServerSyncedDate;
                    mediaObject.Version = vmedia.Version;
                    mediaObject.IsDeleted = vmedia.IsDeleted;
                    mediaObject.MediaType = (int)vmedia.MediaType;
                });
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectManager", "SaveRoutePointMediaObject", e, false);
            }

            return returnId;
        }

        internal void DeleteObjectFromLocalStorage(ViewRoutePointMediaObject vmedia)
        {
            if (vmedia != null)
            {
                try
                {
                    RoutePointMediaObject media = !string.IsNullOrEmpty(vmedia.Id) ? RealmInstance.Find<RoutePointMediaObject>(vmedia.Id) : null;
                    if (media != null)
                    {
                        RealmInstance.Write(() =>
                        {
                            RealmInstance.Remove(media);
                        });
                    }
                }
                catch (Exception e)
                {
                    HandleError.Process("RoutePointMediaObjectManager", "DeleteObjectFromLocalStorage", e, false);
                }
            }
        }

        internal ViewRoutePointMediaObject GetFirstMediaObjectByRouteId(string routeId)
        {
            ViewRoutePointMediaObject resultMedia = new ViewRoutePointMediaObject();
            var point = RealmInstance.All<RoutePoint>().Where(p => p.RouteId == routeId&&!p.IsDeleted).OrderBy(p=>p.CreateDate).ToList().FirstOrDefault();
            if (point != null)
            {
                var media = RealmInstance.All<RoutePointMediaObject>().Where(p => p.RoutePointId == point.RoutePointId&&!p.IsDeleted).FirstOrDefault();
                if (media != null)
                {
                    resultMedia.Load(media.RoutePointMediaObjectId);
                }
            }
            return resultMedia;
        }

        internal IEnumerable<RoutePointMediaObject> GetMediaObjectsByRouteId(string routeId)
        {
            var points = RealmInstance.All<RoutePoint>().Where(p=>p.RouteId == routeId);
            var objects = RealmInstance.All<RoutePointMediaObject>().ToList().Where(m => (points.Any(p => p.RoutePointId == m.RoutePointId)));
            return objects;
        }

        internal IEnumerable<RoutePointMediaObject> GetNotSyncedMediaObjects(bool OnlyPreview)
        {
            if(OnlyPreview)
                return RealmInstance.All<RoutePointMediaObject>().Where(x=>!x.PreviewServerSynced);
            else
                return RealmInstance.All<RoutePointMediaObject>().Where(x => !x.OriginalServerSynced);
        }

        internal RoutePointMediaObject GetMediaObjectById(string mediaId)
        {
            return RealmInstance.All<RoutePointMediaObject>().SingleOrDefault(x => x.RoutePointMediaObjectId == mediaId);
        }

        internal IEnumerable<RoutePointMediaObject> GetMediaObjectsByRoutePointId(string routePointId)
        {
            return RealmInstance.All<RoutePointMediaObject>().Where(x=>x.RoutePointId == routePointId&&!x.IsDeleted);
        }
        public void TryDeleteFile(string routePointMediaObjectId, MediaObjectTypeEnum mediaType, bool isPreview = false)
        {
            string filename = ImagePathManager.GetImagePath(routePointMediaObjectId, mediaType, isPreview);
            if (File.Exists(filename))
            {
                try
                {
                    File.Delete(filename);
                }
                catch (Exception e)
                {
                    HandleError.Process("RoutePointMediaObjectManager", "TryDeleteFile", e, false);
                }
            }
        }
    }
}
