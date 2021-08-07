using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Autofac;
using Newtonsoft.Json;
using QuestHelper.Managers;
using QuestHelper.Model;
using QuestHelper.SharedModelsWS;
using QuestHelper.ViewModel;

namespace QuestHelper.WS
{
    public class TrackRouteRequest
    {
        private readonly IServerRequest _serverRequest = App.Container.Resolve<IServerRequest>();
        private readonly string _authToken;

        public TrackRouteRequest(string authToken)
        {
            _authToken = authToken;
        }

        public async Task<RouteTracking> GetTrackPlacesAsync(string routeId)
        {
            RouteTracking result = new RouteTracking();
            try
            {
                var response = await _serverRequest.HttpRequestGet($"/api/v2/routes/{routeId}/tracks", _authToken);
                result = JsonConvert.DeserializeObject<RouteTracking>(response);
            }
            catch (Exception e)
            {
                HandleError.Process("TrackRouteRequest", "GetTrackPlacesAsync", e, false);
            }

            return result;
        }
        public async Task<bool> SendTrackFileAsync(string trackFilename, string routeId)
        {
            bool result = false;
            try
            {
                string pathToTrackFile = Path.Combine(ImagePathManager.GetTracksDirectory(), trackFilename);
                using (FileStream imageStream = File.Open(pathToTrackFile, FileMode.Open))
                {
                    byte[] fileContent = new byte[imageStream.Length];
                    imageStream.Read(fileContent, 0, Convert.ToInt32(imageStream.Length));
                    result = await _serverRequest.HttpRequestPostFile($"/api/v2/routes/{routeId}/tracks", _authToken, fileContent, trackFilename);
                }
            }
            catch (Exception e)
            {
                HandleError.Process("TrackRouteRequest", "SendTrackFileAsync", e, false);
            }

            return result;
        }

        public ViewTrackPlace[] GetViewTrackPlaces(RouteTracking.RouteTrackingPlace[] places)
        {
            if (places.Length > 0)
            {
                var viewPlaces = places.Select(p => new ViewTrackPlace()
                {
                    Id = p.Id,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude,
                    Name = p.Name,
                    Description = p.Description,
                    Address = p.Address,
                    Category = p.Category,
                    Distance = p.Distance,
                    DateTimeBegin = !p.DateTimeBegin.Equals(DateTime.MinValue)
                        ? new DateTimeOffset(p.DateTimeBegin)
                        : DateTimeOffset.MinValue,
                    DateTimeEnd = !p.DateTimeEnd.Equals(DateTime.MinValue)
                        ? new DateTimeOffset(p.DateTimeEnd)
                        : DateTimeOffset.MinValue,
                    Elevation = 0
                }).ToArray();
                return viewPlaces;
            }

            return new ViewTrackPlace[]
            {
            };
        }
    }
}