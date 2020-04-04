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

        internal List<ViewPoi> GetMyPois(string creatorId)
        {
            var vPois = RealmInstance.All<Poi>().Where(p => !p.IsDeleted && p.CreatorId.Equals(creatorId)).ToList().Select(p => new ViewPoi(p.PoiId));
            return vPois.ToList();
        }

        internal List<ViewPoi> GetAllAvailablePois(string creatorId)
        {
            var vPois = RealmInstance.All<Poi>().Where(p => !p.IsDeleted && (p.CreatorId.Equals(creatorId) || p.IsPublished)).ToList().Select(p => new ViewPoi(p.PoiId));
            return vPois.ToList();
        }

        internal void Delete(string poiId)
        {
            try
            {
                RealmInstance.Write(() =>
                {
                    var poiDb = RealmInstance.Find<Poi>(poiId);
                    if (poiDb != null)
                    {
                        RealmInstance.Remove(poiDb);
                    }
                });
            }
            catch (Exception e)
            {
                HandleError.Process("PoiManager", "Delete", e, false);
            }
        }

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
                    poi.ByRouteId = viewPoi.ByRouteId;
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
        internal ViewPoi GetPoiByRoutePointId(string routePointId)
        {
            var poiDb = RealmInstance.All<Poi>().Where(p => !p.IsDeleted && p.ByRoutePointId.Equals(routePointId)).FirstOrDefault();
            if (poiDb != null)
            {
                return new ViewPoi(poiDb.PoiId);
            }
            return new ViewPoi();
        }

    }
}
