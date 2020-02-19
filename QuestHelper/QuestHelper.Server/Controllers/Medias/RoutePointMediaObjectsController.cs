using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestHelper.Server.Integration;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Controllers.Medias
{
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/[controller]")]
    public class RoutePointMediaObjectsController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();
        private string _pathToMediaCatalog = string.Empty;
        private MediaManager _mediaManager;

        public RoutePointMediaObjectsController()
        {
            _mediaManager = new MediaManager();
            _pathToMediaCatalog = _mediaManager.PathToMediaCatalog;
        }

        [HttpGet("{routePointMediaObjectId}")]
        public IActionResult Get(string routePointMediaObjectId)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            RoutePointMediaObject item;
            using (var db = new ServerDbContext(_dbOptions))
            {
                var publishRoutes = db.Route.Where(r => r.IsPublished && r.IsDeleted == false).Select(r => r.RouteId).ToList();
                var routeAccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                var availablePoints = db.RoutePoint.Where(p => routeAccess.Contains(p.RouteId)||(publishRoutes.Contains(p.RouteId))).Select(p => p.RoutePointId).ToList();
                item = db.RoutePointMediaObject.SingleOrDefault(x => x.RoutePointMediaObjectId == routePointMediaObjectId && availablePoints.Contains(x.RoutePointId));
            }
            return new ObjectResult(item != null ? item : new RoutePointMediaObject());
        }

        [HttpPost]
        public void Post([FromBody]RoutePointMediaObject routeMediaObject)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            using (var db = new ServerDbContext(_dbOptions))
            {
                var routeAccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                var accessGranted = db.RoutePoint.Where(p => routeAccess.Contains(p.RouteId) && p.RoutePointId == routeMediaObject.RoutePointId).Any();
                if (accessGranted)
                {
                    var entity = db.RoutePointMediaObject.Find(routeMediaObject.RoutePointMediaObjectId);
                    bool NeedProcess = routeMediaObject.MediaType == MediaObjectTypeEnum.Audio;
                    if (entity == null)
                    {
                        routeMediaObject.NeedProcess = NeedProcess;
                        db.RoutePointMediaObject.Add(routeMediaObject);
                    }
                    else
                    {
                        routeMediaObject.NeedProcess = NeedProcess;
                        db.Entry(entity).CurrentValues.SetValues(routeMediaObject);
                    }

                    var entityPoint = db.RoutePoint.Find(routeMediaObject.RoutePointId);
                    var entityRoute = db.Route.Find(entityPoint.RouteId);
                    if (entityRoute != null)
                    {
                        entityRoute.VersionsHash = string.Empty;
                        entityRoute.VersionsList = string.Empty;
                        db.Entry(entityRoute).CurrentValues.SetValues(entityRoute);
                    }
                    db.SaveChanges();
                }
                else
                {
                    Response.StatusCode = 400;
                }
            }
        }

        [HttpPost("{routePointId}/{mediaObjectId}/uploadfile")]
        public async Task PostUploadFileAsync(string routePointId, string mediaObjectId, IFormFile file)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            if (file.Length > 0)
            {
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var routeAccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                    var accessGranted = db.RoutePoint.Where(p => routeAccess.Contains(p.RouteId) && p.RoutePointId == routePointId).Any();
                    if (accessGranted)
                    {
                        var entity = db.RoutePointMediaObject.Find(mediaObjectId);
                        if ((entity != null) && file.FileName.Contains(entity.RoutePointMediaObjectId))
                        {
                            using (var stream = new FileStream(Path.Combine(_pathToMediaCatalog, file.FileName), FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            if (file.FileName.Contains("_preview"))
                            {
                                entity.ImagePreviewLoadedToServer = true;
                            }
                            else
                            {
                                entity.ImageLoadedToServer = true;
                            }
                            db.Entry(entity).CurrentValues.SetValues(entity);
                            db.SaveChanges();
                        }
                        else
                        {
                            if (entity == null) throw new Exception($"Media object {mediaObjectId} not found!");
                            throw new Exception($"Media object does not contain filename {file.FileName}");
                        }
                    }
                    else
                    {
                        Response.StatusCode = 400;
                    }
                }
            }
        }

        [HttpGet("{routePointId}/{mediaObjectId}/{fileName}")]
        public IActionResult Get(string routePointId, string mediaObjectId, string fileName)
        {
            DateTime startDate = DateTime.Now;

            string userId = IdentityManager.GetUserId(HttpContext);
            MemoryStream memStream = new MemoryStream();
            using (var db = new ServerDbContext(_dbOptions))
            {
                var publishRoutes = db.Route.Where(r => r.IsPublished && r.IsDeleted == false).Select(r => r.RouteId).ToList();
                var routeAccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                var accessGranted = db.RoutePoint.Where(p => (routeAccess.Contains(p.RouteId) || publishRoutes.Contains(p.RouteId)) && p.RoutePointId == routePointId).Any();
                if (accessGranted)
                {
                    var entity = db.RoutePointMediaObject.Find(mediaObjectId);
                    if ((entity != null) && (!string.IsNullOrEmpty(entity.RoutePointMediaObjectId)))
                    {
                        //ToDo:memory stream не нужен, можно просто отдавать filestream
                        try
                        {
                            _mediaManager.DownloadToStream(memStream, fileName);
                            memStream.Position = 0;
                            return File(memStream, "image/jpeg", fileName);
                        }
                        catch (Exception e)
                        {
                            Response.StatusCode = 404;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Image GET exception: userid:{userId}, file:{fileName}, not found");
                        if (entity == null) throw new Exception($"Media object {mediaObjectId} not found!");
                        throw new Exception($"Media object does not contain filename {fileName}");
                    }
                }
                else
                {
                    Response.StatusCode = 204;
                }
            }

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"Image GET: status {Response.StatusCode}, {userId}, file:{fileName}, delay:{delay.TotalMilliseconds}");

            return new ObjectResult("");
        }

        [HttpGet("{fileName}/imageexist")]
        public void ImageExist(string fileName)
        {
            DateTime startDate = DateTime.Now;

            string userId = IdentityManager.GetUserId(HttpContext);
            if (_mediaManager.MediaFileExist(Path.Combine(_pathToMediaCatalog, fileName)))
                Response.StatusCode = 200;
            else Response.StatusCode = 404;

            TimeSpan delay = DateTime.Now - startDate;
            Console.WriteLine($"Image exist: status {Response.StatusCode}, {userId}, file:{fileName}, delay:{delay.TotalMilliseconds}");
        }

        /*[HttpPost("tryspeechparse")]
        public async Task TrySpeachParseAsync()
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            SpeachToTextProcess speachToTextProcess = new SpeachToTextProcess(_pathToMediaCatalog);
            var result = await speachToTextProcess.TrySpeachParseAllAsync();
            Response.StatusCode  = result ? 200 : 500;
        }*/
    }
}
