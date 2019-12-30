using Microsoft.AppCenter.Crashes;
using QuestHelper.LocalDB.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Managers
{
    public class ImagePreviewManager
    {
        private int _width = 640;
        private int _height = 480;
        private int _quality = 70;

        private byte[] GetPreviewImage(IMediaService mediaService, byte[] byteArrayOriginal, int width, int height, int quality)
        {
            var byteArrayPreview = mediaService.ResizeImage(byteArrayOriginal, width, height, quality);
            return byteArrayPreview;
        }
        public bool CreateImagePreview(string pathToOriginalDirectory, string originalFileName, string pathToPreviewDirectory, string previewFileName)
        {
            bool result = false;
            try
            {
                byte[] originalByteArray = File.ReadAllBytes(pathToOriginalDirectory + "/" + originalFileName);
                if (originalByteArray.Length > 0)
                {
                    ImagePreviewManager previewManager = new ImagePreviewManager();
                    var mediaService = DependencyService.Get<IMediaService>();
                    byte[] imgPreviewByteArray = previewManager.GetPreviewImage(mediaService, originalByteArray, _width, _height, _quality);
                    File.WriteAllBytes(pathToPreviewDirectory + "/" + previewFileName, imgPreviewByteArray);
                    result = true;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e, new Dictionary<string, string> { { "Img", "ImgPreview" }, { originalFileName, previewFileName } });
            }

            return result;
        }
        public bool CreateImagePreview(string mediaId)
        {
            bool result = false;
            string photoNamePreview = ImagePathManager.GetMediaFilename(mediaId, MediaObjectTypeEnum.Image, true);
            string pathToOriginal = ImagePathManager.GetImagePath(mediaId, MediaObjectTypeEnum.Image, false);
            string pathToPreview = ImagePathManager.GetImagePath(mediaId, MediaObjectTypeEnum.Image, true);
            try
            {
                byte[] originalByteArray = File.ReadAllBytes(pathToOriginal);
                if (originalByteArray.Length > 0)
                {
                    ImagePreviewManager previewManager = new ImagePreviewManager();
                    var mediaService = DependencyService.Get<IMediaService>();
                    byte[] imgPreviewByteArray = previewManager.GetPreviewImage(mediaService, originalByteArray, _width, _height, _quality);
                    File.WriteAllBytes(pathToPreview, imgPreviewByteArray);
                    result = true;
                }
            }
            catch (Exception e)
            {
                Crashes.TrackError(e, new Dictionary<string, string> { { "MediaId", "ImgPreview" }, { mediaId, photoNamePreview } });
            }

            return result;
        }

        public int PreviewWidth
        {
            set
            {
                _width = value; 
            }
            get
            {
                return _width;
            }
        }
        public int PreviewHeight
        {
            set
            {
                _height = value;
            }
            get
            {
                return _height;
            }
        }
        public int PreviewQuality
        {
            set
            {
                _quality = value;
            }
            get
            {
                return _quality;
            }
        }
    }
}
