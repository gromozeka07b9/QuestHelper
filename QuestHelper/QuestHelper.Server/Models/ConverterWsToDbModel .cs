using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Models
{
    /// <summary>
    /// Конвертер модели веб-сервиса в модель БД
    /// </summary>
    public class ConverterWsToDbModel
    {
        /// <summary>
        /// Конвертер для модели POI
        /// </summary>
        /// <param name="wsObject"></param>
        /// <returns></returns>
        public static Models.Poi PoiConvert(SharedModelsWS.Poi wsObject)
        {
            Models.Poi dbObject = new Models.Poi();
            if (wsObject != null)
            {
                dbObject.Name = wsObject.Name;
                dbObject.ByRoutePointId = wsObject.ByRoutePointId;
                dbObject.UpdateDate = wsObject.UpdateDate;
                dbObject.Latitude = wsObject.Latitude;
                dbObject.Longitude = wsObject.Longitude;
                dbObject.Address = wsObject.Address;
                dbObject.Description = wsObject.Description;
                dbObject.PoiId = wsObject.Id;
                dbObject.CreateDate = wsObject.CreateDate.UtcDateTime;
                dbObject.IsDeleted = wsObject.IsDeleted;
                dbObject.CreatorId = wsObject.CreatorId;
                dbObject.IsPublished = wsObject.IsPublished;
                dbObject.ImgFilename = wsObject.ImgFilename;
                dbObject.Version = wsObject.Version;
            }

            return dbObject;
        }

    }
}
