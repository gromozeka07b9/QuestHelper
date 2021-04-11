using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Autofac;
using QuestHelper.Managers;

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
        /*private async Task<bool> TryToSendFileAsync(string pathToMediaFile, string nameMediafile, string routePointId, string routePointMediaObjectId)
        {
            bool result = false;
            try
            {
                using (Stream image = File.Open(pathToMediaFile, FileMode.Open))
                {
                    using (HttpContent content = new StreamContent(image))
                    {
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data") { Name = "file", FileName = nameMediafile };
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
                            using (var formData = new MultipartFormDataContent())
                            {
                                formData.Add(content);
                                HttpResponseMessage response = await client.PostAsync($"{ this._hostUrl }/routepointmediaobjects/{ routePointId }/{ routePointMediaObjectId }/uploadfile", formData);
                                LastHttpStatusCode = response.StatusCode;
                                result = response.IsSuccessStatusCode;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleError.Process("RoutePointMediaObjectApiRequest", "TryToSendFileAsync", e, false);
                result = false;
            }

            return result;
        }*/

    }
}