using Newtonsoft.Json.Linq;
using QuestHelper.Model;
using QuestHelper.WS;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace QuestHelper.Managers.Sync
{
    public class SyncRouteBase
    {
        public async System.Threading.Tasks.Task<bool> UploadAsync<T>(List<string> jsonStructureForUpload, IUploadable<T> api)
        {
            bool result = true;
            StringBuilder sbErrors = new StringBuilder();
            string syncType = typeof(T).ToString();
            foreach (string objectJson in jsonStructureForUpload)
            {
                bool resultId = await api.UploadToServerAsync(objectJson);
                bool AuthRequired = (api.GetLastHttpStatusCode() == HttpStatusCode.Forbidden || api.GetLastHttpStatusCode() == HttpStatusCode.Unauthorized);
                if (!resultId)
                {
                    sbErrors.AppendLine($"uploading {syncType}:{objectJson}");
                    result = resultId;
                }
            }
            //_errorReport += sbErrors.ToString();
            return result;
        }

        protected List<string> GetRouteJsonStructure(ViewRoute viewRoute)
        {
            List<string> jsonStructures = new List<string>();
            if (viewRoute != null)
            {
                JObject jsonObject = JObject.FromObject(new
                {
                    RouteId = viewRoute.RouteId,
                    Name = viewRoute.Name,
                    CreateDate = viewRoute.CreateDate.DateTime,
                    Version = viewRoute.Version,
                    IsPublished = viewRoute.IsPublished,
                    CreatorId = viewRoute.CreatorId,
                    UserId = 0
                });
                jsonStructures.Add(jsonObject.ToString());
            }
            return jsonStructures;
        }

        internal List<string> GetJsonStructuresPoints(List<ViewRoutePoint> pointsForUpload)
        {
            List<string> jsonStructures = new List<string>();

            foreach (var viewPoint in pointsForUpload)
            {
                if (viewPoint.CreateDate.Year > 2000)
                {
                    JObject jsonObject = JObject.FromObject(new
                    {
                        viewPoint.RoutePointId,
                        viewPoint.RouteId,
                        viewPoint.Name,
                        CreateDate = viewPoint.CreateDate.DateTime,
                        UpdateDate = viewPoint.CreateDate.DateTime,
                        UpdatedUserId = "",
                        viewPoint.Latitude,
                        viewPoint.Longitude,
                        viewPoint.Address,
                        viewPoint.Description,
                        viewPoint.Version,
                        viewPoint.IsDeleted
                    });
                    jsonStructures.Add(jsonObject.ToString());
                }
            }

            return jsonStructures;
        }
        protected List<string> GetJsonStructuresMedias(List<ViewRoutePointMediaObject> mediasForUpload)
        {
            List<string> jsonStructures = new List<string>();

            foreach (var viewMedia in mediasForUpload)
            {
                if (viewMedia != null)
                {
                    JObject jsonObject = JObject.FromObject(new
                    {
                        viewMedia.RoutePointMediaObjectId,
                        viewMedia.RoutePointId,
                        viewMedia.Version,
                        viewMedia.IsDeleted,
                        viewMedia.MediaType
                    });
                    jsonStructures.Add(jsonObject.ToString());
                }
            }

            return jsonStructures;
        }

        protected void refreshHashForRoute(ViewRoute viewRoute, StringBuilder versions)
        {
            viewRoute.ObjVerHash = HashManager.Generate(versions.ToString());
            viewRoute.Save();
        }

        protected void refreshHashForRouteByString(ViewRoute viewRoute, string hash)
        {
            viewRoute.ObjVerHash = hash;
            viewRoute.Save();
        }


    }
}