using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AppCenter.Crashes;

namespace QuestHelper.WS
{
    public class AzureBlobRequest : ICloudStoreFiles
    {
        public AzureBlobRequest()
        {

        }

        public bool SendFile(string imgFilePath, string imgFileName)
        {
            bool result = false;
            try
            {
                var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=questhelperblob;AccountKey=0i3of0RpMeMuIo3OOq2mqPxLPTfuCF0gWt/6/dh3SZXT2fT1JexrQJLUKOOhwmYTEjFmctXUJMSp1JAk8iAjTA==;EndpointSuffix=core.windows.net");
                var client = account.CreateCloudBlobClient();
                var container = client.GetContainerReference("questhelperblob");
                container.CreateIfNotExistsAsync();
                CloudBlockBlob blob = container.GetBlockBlobReference(imgFileName);
                blob.UploadFromFileAsync(imgFilePath + "/" + imgFileName);
                result = true;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e, new Dictionary<string, string> { { "Storage", "Img" }, { "AzureBlobRequestSendFile", imgFilePath + "/" + imgFileName } });
            }
            return result;
        }

        public void ReceiveFile(string fileId, string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
