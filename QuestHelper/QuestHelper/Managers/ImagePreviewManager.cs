using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Managers
{
    public class ImagePreviewManager
    {
        public byte[] GetPreviewImage(IMediaService mediaService, byte[] byteArrayOriginal, int width, int height)
        {
            var byteArrayPreview = mediaService.ResizeImage(byteArrayOriginal, width, height);
            return byteArrayPreview;
        }

        public void CreateImagePreview(string pathToDirectory, string originalFileName, string previewFileName)
        {
            try
            {
                byte[] originalByteArray = File.ReadAllBytes(pathToDirectory + "/" + originalFileName);
                if (originalByteArray.Length > 0)
                {
                    ImagePreviewManager previewManager = new ImagePreviewManager();
                    var mediaService = DependencyService.Get<IMediaService>();
                    byte[] imgPreviewByteArray = previewManager.GetPreviewImage(mediaService, originalByteArray, 640, 480);
                    File.WriteAllBytes(pathToDirectory + "/" + previewFileName, imgPreviewByteArray);
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e, new Dictionary<string, string> { { "Img", "ImgPreview" }, { pathToDirectory + "/" + originalFileName, previewFileName } });
            }
            /*using (var stream = originalFile.GetStream())
            {
                byte[] originalByteArray = new byte[stream.Length];
                int resultRead = stream.Read(originalByteArray, 0, originalByteArray.Length);
                if (resultRead > 0)
                {
                    ImagePreviewManager previewManager = new ImagePreviewManager();
                    byte[] imgPreviewByteArray = previewManager.GetPreviewImage(mediaService, originalByteArray, 640, 480);
                    FileInfo info = new FileInfo(originalFilePath);
                    File.WriteAllBytes(info.DirectoryName + "/" + previewFileName, imgPreviewByteArray);
                }
            }*/
        }
    }
}
