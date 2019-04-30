﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using QuestHelper.Server.Managers;
using QuestHelper.Server.Models;
using Route = Microsoft.AspNetCore.Routing.Route;

namespace QuestHelper.Server.Controllers.Medias
{
    [Authorize]
    [ServiceFilter(typeof(RequestFilter))]
    [Route("api/routepointmediaobjects/[controller]")]
    public class SyncController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpPost]
        public IActionResult Post([FromBody]SyncObjectStatus syncObject)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            SyncObjectStatus report = new SyncObjectStatus();
            if (syncObject.Statuses != null)
            {
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var publishRoutes = db.Route.Where(r => r.IsPublished).Select(r => r.RouteId).ToList();
                    var routeAccess = db.RouteAccess.Where(u => u.UserId == userId).Select(u => u.RouteId).ToList();
                    var availablePoints = db.RoutePoint.Where(p => (routeAccess.Contains(p.RouteId) || publishRoutes.Contains(p.RouteId))).Select(p=>p.RoutePointId).ToList();
                    var syncIds = syncObject.Statuses.Select(t => t.ObjectId);
                    var dbObjects = db.RoutePointMediaObject.Where(r => syncIds.Contains(r.RoutePointMediaObjectId) && availablePoints.Contains(r.RoutePointId));
                    foreach (var clientVersion in syncObject.Statuses)
                    {
                        var dbObject = dbObjects.SingleOrDefault(r => r.RoutePointMediaObjectId == clientVersion.ObjectId);
                        if (dbObject != null)
                        {
                            if (dbObject.Version != clientVersion.Version)
                                report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = dbObject.RoutePointMediaObjectId, Version = dbObject.Version });
                        }
                        else
                        {
                            report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = clientVersion.ObjectId, Version = 0 });
                        }
                    }
                    var dbNewObjects = db.RoutePointMediaObject.Where(r => !syncIds.Contains(r.RoutePointMediaObjectId) && availablePoints.Contains(r.RoutePointId) && r.ImageLoadedToServer && r.ImagePreviewLoadedToServer);
                    foreach (var newObject in dbNewObjects)
                    {
                        report.Statuses.Add(new SyncObjectStatus.ObjectStatus { ObjectId = newObject.RoutePointMediaObjectId, Version = newObject.Version });
                    }
                }
            }
            return new ObjectResult(report);
        }

        [HttpPost("imagestatus")]
        public async Task<IActionResult> ImagesStatusAsync([FromBody]ImagesServerStatus imagesClient)
        {
            string userId = IdentityManager.GetUserId(HttpContext);
            throw new Exception("azure blob is offline");
            ImagesServerStatus status = new ImagesServerStatus();
            using (var db = new ServerDbContext(_dbOptions))
            {
                var blobContainer = await GetCloudBlobContainer();
                foreach (var image in imagesClient.Images)
                {
                    string imageNameOriginal = image.Name.ToLower().Replace("_preview", "").Replace("img_", "").Trim();
                    var imageNameParts = imageNameOriginal.Split('.');
                    if (imageNameParts.Length > 0)
                    {
                        string imageName = imageNameParts[0];
                        var mediaObject = db.RoutePointMediaObject.Where(m => m.RoutePointMediaObjectId == imageName).SingleOrDefault();
                        if (mediaObject != null)
                        {
                            if (image.Name.Contains("_preview"))
                            {
                                image.OnServer = mediaObject.ImagePreviewLoadedToServer;
                                if (!mediaObject.ImagePreviewLoadedToServer)
                                {
                                    var blob = blobContainer.GetBlockBlobReference(image.Name);
                                    image.OnServer = await blob.ExistsAsync();
                                }
                            }
                            else
                            {
                                image.OnServer = mediaObject.ImageLoadedToServer;
                                if (!mediaObject.ImageLoadedToServer)
                                {
                                    var blob = blobContainer.GetBlockBlobReference(image.Name);
                                    image.OnServer = await blob.ExistsAsync();
                                }
                            }
                        }
                        status.Images.Add(image);
                    }
                }
            }

            return new ObjectResult(status);
        }
        private async Task<CloudBlobContainer> GetCloudBlobContainer()
        {
            string blobConnectionString = System.Environment.ExpandEnvironmentVariables("%GoshBlobStoreConnectionString%");
            if (string.IsNullOrEmpty(blobConnectionString))
                throw new Exception("Error reading blob connection string!");
            var account = CloudStorageAccount.Parse(blobConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference("questhelperblob");
            await container.CreateIfNotExistsAsync();
            return container;
        }

    }
}
