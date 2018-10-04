using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Blob;

namespace QuestHelper.WS
{
    public class AzureBlobRequest
    {
        public AzureBlobRequest()
        {

        }

        public void SendFile(string imgFilePath, string imgFileName)
        {
            var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=questhelperblob;AccountKey=0i3of0RpMeMuIo3OOq2mqPxLPTfuCF0gWt/6/dh3SZXT2fT1JexrQJLUKOOhwmYTEjFmctXUJMSp1JAk8iAjTA==;EndpointSuffix=core.windows.net");
            var client = account.CreateCloudBlobClient();
            var container = client.GetContainerReference("questhelperblob");
            container.CreateIfNotExistsAsync();
            CloudBlockBlob blob = container.GetBlockBlobReference(imgFileName);
            blob.UploadFromFileAsync(imgFilePath + "/" + imgFileName);
        }

        public void ReceiveBlob()
        {

        }
    }
}
