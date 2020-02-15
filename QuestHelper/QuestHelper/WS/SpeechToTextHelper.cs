using Microsoft.AppCenter.Analytics;
using QuestHelper.Managers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static QuestHelper.WS.AccountApiRequest;

namespace QuestHelper.WS
{
    public class SpeechToTextHelper
    {
        private string _hostUrl = "http://igosh.pro/api";
        private string _authToken = string.Empty;

        public SpeechToTextHelper(string authToken)
        {
            _authToken = authToken;
        }

        public HttpStatusCode LastHttpStatusCode { get; private set; }

        public async Task<string> TryRecognizeAudioAsync(string mediaObjectId)
        {
            string result = string.Empty;
            ApiRequest api = new ApiRequest();
            try
            {
                result = await api.HttpRequestGET($"{this._hostUrl}/SpeechToText/parse/routepointmediaobject/{mediaObjectId}", _authToken);
                LastHttpStatusCode = api.LastHttpStatusCode;
            }
            catch (Exception e)
            {
                HandleError.Process("SpeechToTextHelper", "TryRecognizeAudioAsync", e, false);
            }
            return result;
        }
    }
}
