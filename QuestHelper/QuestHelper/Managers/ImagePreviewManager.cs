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
        private ImageQualityType _previewQuality = ImageQualityType.MiddleSizeHiQuality;

        public ImagePreviewManager()
        {
            PreviewQualityType = ImageQualityType.MiddleSizeHiQuality;
        }

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

        public ImageQualityType PreviewQualityType
        {
            set
            {
                if(value != _previewQuality)
                {
                    switch (value)
                    {
                        case ImageQualityType.MiddleSizeHiQuality:
                            {
                                _width = 640;
                                _height = 480;
                                _quality = 70;
                            };break;
                        case ImageQualityType.SmallSizeMiddleQuality:
                            {
                                _width = 320;
                                _height = 240;
                                _quality = 40;
                            }; break;
                        case ImageQualityType.OriginalSizeLowQuality:
                            {
                                _width = 0;
                                _height = 0;
                                _quality = 40;
                            }; break;
                        case ImageQualityType.MinimumSizeHiQuality:
                            {
                                _width = 240;
                                _height = 200;
                                _quality = 80;
                            }; break;
                        default:
                            {

                            };break;
                    }
                }
            }
            get
            {
                return _previewQuality;
            }
        }
    }

    public enum ImageQualityType
    {
        //обычное превью
        MiddleSizeHiQuality,
        //для меток на карте
        SmallSizeMiddleQuality,
        //для оригинальных изображений, размер тот же
        OriginalSizeLowQuality,
        //превью для построителя маршрутов
        MinimumSizeHiQuality
    }
}

/*
MinimumSizeHiQuality
  240x200x80
1. 6.3кб
2. 9.2кб
3. 7.8кб
  320x240x80
1. 8.2кб
2. 12.3кб
3. 10.5кб

MiddleSizeHiQuality
640x480x70
1. 18.4кб
2. 30кб
3. 24кб
*/