using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuestHelper.LocalDB.Model;
using QuestHelper.Model;
using QuestHelper.SharedModelsWS;
using Realms;

namespace QuestHelper.Managers
{
    public class TrackFileManager : RealmInstanceMaker
    {
        public IEnumerable<Tuple<double?, double?>> GetTrackByRoute(string routeId)
        {
            var collectionRealm = RealmInstance.All<RouteTrackPlace>().Where(place => place.RouteTrackId.Equals(routeId))
                    .OrderBy(place=>place.DateTimeBegin).ToList().Select(place => new
                    {
                        place.Latitude, place.Longitude
                    });
            return collectionRealm.Select(x=>new Tuple<double?, double?>(x.Latitude, x.Longitude));
        }
        
        public IEnumerable<FileInfo> GetTrackFilesFromDirectory()
        {
            IEnumerable<FileInfo> files = new List<FileInfo>();
            DirectoryInfo directory = new DirectoryInfo(ImagePathManager.GetTracksDirectory());
            try
            {
                files =  directory.GetFiles("*")
                    .Where(f => f.Name.EndsWith(".kml") || f.Name.EndsWith(".gpx"));
                
            }
            catch (Exception e)
            {
                HandleError.Process("TrackFileManager", "GetMediaFilesFromDirectory", e, false, directory.FullName);
            }

            return files;
        }

        public bool SaveTrack(string routeId, ViewTrackPlace[] trackResponsePlaces)
        {
            bool result = false;
            try
            {
                RealmInstance.Write(() =>
                {
                    RealmInstance.RemoveRange(RealmInstance.All<RouteTrackPlace>()
                        .Where(p => p.RouteTrackId.Equals(routeId)));
                    foreach (var place in trackResponsePlaces)
                    {
                        RealmInstance.Add(new RouteTrackPlace()
                        {
                            RouteTrackId = routeId,
                            Id = place.Id,
                            Name = place.Name,
                            Description = place.Description,
                            DateTimeBegin = place.DateTimeBegin,
                            DateTimeEnd = place.DateTimeEnd,
                            Latitude = place.Latitude,
                            Longitude = place.Longitude,
                            Address = place.Address,
                            Category = place.Category,
                            Distance = place.Distance,
                            Elevation = place.Elevation
                        });
                    }
                });
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("TrackFileManager", "SaveTrack", e, false);
            }

            return result;
        }

        public bool RemoveAllTracksFromRoute(string routeId)
        {
            bool result = false;
            try
            {
                RealmInstance.Write(() =>
                {
                    RealmInstance.RemoveRange(RealmInstance.All<RouteTrackPlace>()
                        .Where(p => p.RouteTrackId.Equals(routeId)));
                });
                result = true;
            }
            catch (Exception e)
            {
                HandleError.Process("TrackFileManager", "RemoveAllTracksFromRoute", e, false);
            }

            return result;
        }
    }
}