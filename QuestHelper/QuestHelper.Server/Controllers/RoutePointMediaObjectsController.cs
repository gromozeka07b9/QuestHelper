using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers
{
    [Route("api/[controller]")]
    public class RoutePointMediaObjectsController : Controller
    {
        [HttpGet("{routePointId}")]
        public IActionResult Get(string routePointId)
        {
            List<RoutePointMediaObject> items = new List<RoutePointMediaObject>();
            using (var db = new ServerDbContext())
            {
                items = db.RoutePointMediaObject.Where(x=>x.RoutePointId == routePointId).ToList();
            }
            return new ObjectResult(items);
        }

        [HttpPost]
        public void Post([FromBody]RoutePointMediaObject routeMediaObject)
        {
            using (var db = new ServerDbContext())
            {
                var entity = db.RoutePointMediaObject.Find(routeMediaObject.RoutePointMediaObjectId);
                if (entity == null)
                {
                    db.RoutePointMediaObject.Add(routeMediaObject);
                }
                else
                {
                    db.Entry(entity).CurrentValues.SetValues(routeMediaObject);
                }
                db.SaveChanges();
            }
        }

        [HttpPost("{routePointId}/{mediaObjectId}/uploadfile")]
        public async Task PostUploadFileAsync(string routePointId, string mediaObjectId, IFormFile file)
        {
            if (file.Length > 0)
            {
                using (var db = new ServerDbContext())
                {
                    var entity = db.RoutePointMediaObject.Find(mediaObjectId);
                    if ((entity != null)&&((entity.FileName == file.FileName) || (entity.FileNamePreview == file.FileName)))
                    {
                        using (Stream stream = file.OpenReadStream())
                        {
                            var blobContainer = await GetCloudBlobContainer();
                            var blob = blobContainer.GetBlockBlobReference(file.FileName);
                            await blob.UploadFromStreamAsync(stream);
                        }
                    }
                    else
                    {
                        if(entity == null) throw new Exception($"Media object {mediaObjectId} not found!");
                        throw new Exception($"Media object does not contain filename {file.FileName}");
                    }
                }
            }
        }

        [HttpGet("{routePointId}/{mediaObjectId}/{fileName}")]
        public async Task<IActionResult> GetAsync(string routePointId, string mediaObjectId, string fileName)
        {
            Stream memStream = new MemoryStream();
            using (var db = new ServerDbContext())
            {
                var entity = db.RoutePointMediaObject.Find(mediaObjectId);
                if ((entity != null) && ((entity.FileName == fileName) || (entity.FileNamePreview == fileName)))
                {
                    var blobContainer = await GetCloudBlobContainer();
                    var blob = blobContainer.GetBlockBlobReference(fileName);
                    await blob.DownloadToStreamAsync(memStream);
                }
                else
                {
                    if (entity == null) throw new Exception($"Media object {mediaObjectId} not found!");
                    throw new Exception($"Media object does not contain filename {fileName}");
                }
            }

            memStream.Position = 0;
            return File(memStream, "image/jpeg", fileName);
        }

        private async Task<CloudBlobContainer> GetCloudBlobContainer()
        {
            var account = CloudStorageAccount.Parse("xxx");
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference("questhelperblob");
            await container.CreateIfNotExistsAsync();
            return container;
        }

    }
}
