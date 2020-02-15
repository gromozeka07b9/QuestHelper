using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace QuestHelper.Server.Integration
{
    /// <summary>
    /// Запросы к API Yandex для распознавания аудио
    /// </summary>
    public class SpeechToTextRequest
    {
        private string _yandexFolderId = string.Empty;
        private string _yandexApiKey = string.Empty;
        private string _yandexSpeechUrl = string.Empty;

        public SpeechToTextRequest()
        {
            _yandexFolderId = System.Environment.GetEnvironmentVariable("GoshYandexFolderId");
            _yandexApiKey = System.Environment.GetEnvironmentVariable("GoshYandexApiKey");
            _yandexSpeechUrl = $"https://stt.api.cloud.yandex.net/speech/v1/stt:recognize?folderId={ _yandexFolderId }";
        }

        /// <summary>
        /// Распознавание одного аудио файла
        /// </summary>
        /// <param name="pathToAudioFile"></param>
        /// <returns></returns>
        public async Task<RequestResult> GetTextAsync(string pathToAudioFile)
        {
            DateTime startDate = DateTime.Now;

            RequestResult result = new RequestResult();
            result.AudioFile = pathToAudioFile;
            using (Stream audioFile = new FileStream(pathToAudioFile, FileMode.Open, FileAccess.Read))
            {
                using (HttpContent content = new StreamContent(audioFile))
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", $"Api-Key {_yandexApiKey}");
                        var response = await client.PostAsync($"{ _yandexSpeechUrl }", content);
                        if (response.IsSuccessStatusCode)
                        {
                            result.StatusCode = 200;
                            string wsResponse = await response.Content.ReadAsStringAsync();
                            var definitionJson = new { Result = string.Empty};
                            result.Text = JsonConvert.DeserializeAnonymousType(wsResponse, definitionJson).Result;
                        }
                        else
                        {
                            result.StatusCode = (int)response.StatusCode;
                        }
                    }
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            result.Delay = delay;
            Console.WriteLine($"Yandex speech parse: result:{result.StatusCode}, delay:{result.Delay.Milliseconds}, file:{result.AudioFile}");

            return result;
        }

        public class RequestResult
        {
            public int StatusCode { get; internal set; }
            public string Text { get; internal set; }
            public TimeSpan Delay { get; internal set; }
            public string AudioFile { get; internal set; }
        }
    }
}
