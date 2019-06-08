using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Integration
{
    public class SpeachToTextProcess
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
        private string _pathToMediaCatalog = string.Empty;

        public SpeachToTextProcess(string pathToMediaCatalog)
        {
            _pathToMediaCatalog = pathToMediaCatalog;
        }

        public async Task<bool> TrySpeachParseAsync()
        {
            bool result = true;
            SpeachToTextRequest speachToText = new SpeachToTextRequest();
            try
            {
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var audioObjects = db.RoutePointMediaObject.Where(m => m.MediaType == MediaObjectTypeEnum.Audio && m.NeedProcess && !m.Processed);
                    foreach (var audioObj in audioObjects)
                    {
                        var resultSpeachParsing = await speachToText.GetTextAsync(Path.Combine(_pathToMediaCatalog, $"audio_{audioObj.RoutePointMediaObjectId}.3gp"));
                        if (resultSpeachParsing.StatusCode == 200)
                        {
                            var entityRoutePoint = db.RoutePoint.Find(audioObj.RoutePointId);
                            if (entityRoutePoint != null)
                            {
                                entityRoutePoint.Description +=
                                    $"{Environment.NewLine}Yandex speach service response:{Environment.NewLine}{resultSpeachParsing.Text}";
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
                            Console.WriteLine($"Yandex audio parser status code:{resultSpeachParsing.StatusCode}");
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
        }
    }
}
