﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AppCenter.Crashes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using QuestHelper.Server.Models;

namespace QuestHelper.Server.Controllers.Medias
{
    [Route("api/[controller]")]
    public class RoutePointMediaObjectsController : Controller
    {
        private DbContextOptions<ServerDbContext> _dbOptions = ServerDbContext.GetOptionsContextDbServer();

        [HttpGet("{routePointMediaObjectId}")]
        public IActionResult Get(string routePointMediaObjectId)
        {
            RoutePointMediaObject item = new RoutePointMediaObject();
            using (var db = new ServerDbContext(_dbOptions))
            {
                item = db.RoutePointMediaObject.SingleOrDefault(x => x.RoutePointMediaObjectId == routePointMediaObjectId);
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public void Post([FromBody]RoutePointMediaObject routeMediaObject)
        {
            using (var db = new ServerDbContext(_dbOptions))
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
                using (var db = new ServerDbContext(_dbOptions))
                {
                    var entity = db.RoutePointMediaObject.Find(mediaObjectId);
                    if ((entity != null) && file.FileName.Contains(entity.RoutePointMediaObjectId))
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
            using (var db = new ServerDbContext(_dbOptions))
            {
                var entity = db.RoutePointMediaObject.Find(mediaObjectId);
                if ((entity != null)&&(!string.IsNullOrEmpty(entity.FileName)|| !string.IsNullOrEmpty(entity.FileNamePreview)))
                {
                    //есть фото в старом формате, перенесем фото в blobstore и дальше будем с ним работать как с blob
                    string oldFilename = !string.IsNullOrEmpty(entity.FileName) ? entity.FileName : entity.FileNamePreview;
                    var oldFilenameArray = oldFilename.Split('/');
                    if (oldFilenameArray.Length > 0)
                    {
                        oldFilename = oldFilenameArray[oldFilenameArray.Length - 1];
                    }
                    var blobContainer = await GetCloudBlobContainer();
                    var blob = blobContainer.GetBlockBlobReference(oldFilename);
                    var newBlob = blobContainer.GetBlockBlobReference(fileName);
                    await newBlob.StartCopyAsync(blob);
                    await blob.DeleteIfExistsAsync();
                    entity.PreviewImage = null;
                    entity.FileName = null;
                    entity.FileNamePreview = null;
                    db.Entry(entity).CurrentValues.SetValues(entity);
                    db.SaveChanges();
                }
                if ((entity != null) && (!string.IsNullOrEmpty(entity.RoutePointMediaObjectId)))
                {
                    var blobContainer = await GetCloudBlobContainer();
                    try
                    {
                        var blob = blobContainer.GetBlockBlobReference(fileName);
                        await blob.DownloadToStreamAsync(memStream);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Blob store does not contain filename {fileName}");
                    }
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
            string blobConnectionString = System.Environment.ExpandEnvironmentVariables("%GoshBlobStoreConnectionString%");
            if(string.IsNullOrEmpty(blobConnectionString))
                throw new Exception("Error reading blob connection string!");
            var account = CloudStorageAccount.Parse(blobConnectionString);
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference("questhelperblob");
            await container.CreateIfNotExistsAsync();
            return container;
        }

    }
}