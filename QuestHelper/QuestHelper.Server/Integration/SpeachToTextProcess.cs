using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Controllers.SpeechToText;
using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;
using static QuestHelper.Server.Integration.SpeechToTextRequest;

namespace QuestHelper.Server.Integration
{
    /// <summary>
    /// Распознавание аудиофайлов в текст
    /// </summary>
    public class SpeechToTextProcess
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
        private string _pathToMediaCatalog = string.Empty;

        /// <summary>
        /// Параметр - путь к каталогу сервера, где лежат файлы
        /// </summary>
        /// <param name="pathToMediaCatalog"></param>
        public SpeechToTextProcess(string pathToMediaCatalog)
        {
            _pathToMediaCatalog = pathToMediaCatalog;
        }

        /// <summary>
        /// Распознавание всех ранее не обработанных файлов, без привязки к маршруту
        /// </summary>
        /// <returns></returns>
        /*public async Task<bool> TrySpeechParseAllAsync()
        {
            bool result = true;
            SpeechToTextRequest speechToText = new SpeechToTextRequest();
            try
            {
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var audioObjects = db.RoutePointMediaObject.Where(m => m.MediaType == MediaObjectTypeEnum.Audio && m.NeedProcess && !m.Processed && !m.IsDeleted);
                    foreach (var audioObj in audioObjects)
                    {
                        var resultSpeechParsing = await speechToText.GetTextAsync(Path.Combine(_pathToMediaCatalog, $"audio_{audioObj.RoutePointMediaObjectId}.3gp"));
                        if (resultSpeechParsing.StatusCode == 200)
                        {
                            RawTextCleaner textCleaner = new RawTextCleaner();
                            string recognizedText = textCleaner.Clean(resultSpeechParsing.Text);
                            var entityRoutePoint = db.RoutePoint.Find(audioObj.RoutePointId);
                            if (entityRoutePoint != null)
                            {
                                if (string.IsNullOrEmpty(entityRoutePoint.Description))
                                    entityRoutePoint.Description = recognizedText;
                                else
                                    entityRoutePoint.Description += $"{Environment.NewLine}{recognizedText}";

                                entityRoutePoint.Version++;
                                db.Entry(entityRoutePoint).CurrentValues.SetValues(entityRoutePoint);
                                var entityMediaObject = db.RoutePointMediaObject.Find(audioObj.RoutePointMediaObjectId);
                                entityMediaObject.Processed = true;
                                entityMediaObject.ProcessResultText = "ok";
                                db.Entry(entityMediaObject).CurrentValues.SetValues(entityMediaObject);
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Yandex audio parser status code:{resultSpeechParsing.StatusCode}");
                            result = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }*/

        /// <summary>
        /// Распознавание конкретного media файла
        /// </summary>
        /// <param name="mediaId"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<SpeechParseResult> TrySpeechParseMediaAsync(string mediaId)
        {
            SpeechParseResult resultParse = new SpeechParseResult();
            SpeechToTextRequest speechToText = new SpeechToTextRequest();

            try
            {
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var audioObject = db.RoutePointMediaObject.Where(m => m.RoutePointMediaObjectId.Equals(mediaId)).FirstOrDefault();
                    if(audioObject != null)
                    {
                        if (string.IsNullOrEmpty(audioObject.ProcessResultText))
                        {
                            string mediaFilePath = Path.Combine(_pathToMediaCatalog, $"audio_{audioObject.RoutePointMediaObjectId}.3gp");
                            if (File.Exists(mediaFilePath))
                            {
                                string outputFileName = Path.Combine(Path.GetTempPath(), $"{audioObject.RoutePointMediaObjectId}.ogg");

                                var audioConverter = new Audio3gpToOggConverter();
                                bool successConvert = await audioConverter.ConvertAsync(mediaFilePath, outputFileName);
                                if (successConvert)
                                {
                                    var resultSpeechParsing = await speechToText.GetTextAsync(outputFileName);
                                    if (resultSpeechParsing.StatusCode == 200)
                                    {
                                        RawTextCleaner textCleaner = new RawTextCleaner();
                                        resultParse.Result = true;
                                        resultParse.Text = textCleaner.Clean(resultSpeechParsing.Text);
                                        try
                                        {
                                            var entityMediaObject = db.RoutePointMediaObject.Find(audioObject.RoutePointMediaObjectId);
                                            entityMediaObject.Processed = true;
                                            entityMediaObject.ProcessResultText = resultParse.Text;
                                            db.Entry(entityMediaObject).CurrentValues.SetValues(entityMediaObject);
                                            db.SaveChanges();
                                        }
                                        catch (Exception e)
                                        {
                                            Console.Write("Error save data:" + e.Message);
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception($"Error while request Yandex service:{resultSpeechParsing.StatusCode}");
                                    }
                                }
                                else
                                {
                                    throw new Exception($"Error while convert 3gp to Ogg:{mediaFilePath}");
                                }
                            }
                            else
                            {
                                throw new FileNotFoundException($"Media file not found:{mediaFilePath}");
                            }
                        }
                        else
                        {
                            resultParse.Result = true;
                            resultParse.Text = audioObject.ProcessResultText;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return resultParse;
        }

        public class SpeechParseResult
        {
            public bool Result { get; set; }
            public string Text { get; set; }
        }
    }
}
