using Microsoft.AppCenter.Analytics;
using Plugin.Media;
using Plugin.Media.Abstractions;
using QuestHelper.LocalDB.Model;
using QuestHelper.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QuestHelper.Managers
{
    public class ImageManager : IImageManager
    {
        private ImageQualityType _previewImageQuality;

        public ImageManager()
        {
            _previewImageQuality = ImageQualityType.Q640x480x70;
        }

        public ImageQualityType PreviewImageQuality
        {
            set
            {
                if(value != _previewImageQuality)
                {
                    _previewImageQuality = value;
                }
            }
            get
            {
                return _previewImageQuality;
            }
        }

        public (bool getMetadataPhotoResult, Model.GpsCoordinates imageGpsCoordinates) GetPhoto(string mediaId, string photoFullPath)
        {
            bool getMetadataPhotoResult = false;
            Model.GpsCoordinates imageInfo = new Model.GpsCoordinates();

            string imgPathDirectory = ImagePathManager.GetPicturesDirectory();

            FileInfo originalFileInfo = new FileInfo(photoFullPath);
            ImagePreviewManager preview = new ImagePreviewManager();
            preview.PreviewQualityType = ImageQualityType.Q320x240x40;
            if (preview.CreateImagePreview(originalFileInfo.DirectoryName, originalFileInfo.Name, imgPathDirectory, ImagePathManager.GetMediaFilename(mediaId, MediaObjectTypeEnum.Image, true)))
            {
                ExifManager exif = new ExifManager();
                imageInfo = exif.GetCoordinates(photoFullPath);
                getMetadataPhotoResult = true;
            }
            else
            {
                Analytics.TrackEvent("ImageManager: error create preview for auto route", new Dictionary<string, string> { { "mediaId", mediaId } });
            }
            
            return (getMetadataPhotoResult, imageInfo);
        }


        public async Task<(bool pickPhotoResult, string newMediaId, Model.GpsCoordinates imageGpsCoordinates)> PickPhotoAsync()
        {
            bool pickPhotoResult = false;
            Model.GpsCoordinates imageGpsCoordinates = new Model.GpsCoordinates();
            string mediaId = Guid.NewGuid().ToString();
            await CrossMedia.Current.Initialize();
            if (CrossMedia.Current.IsPickPhotoSupported)
            {
                MediaFile photoPicked = null;

                PermissionManager permissions = new PermissionManager();
                if (await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Photos, CommonResource.RoutePoint_RightNeedToPickPhoto))
                {
                    try
                    {
                        photoPicked = await CrossMedia.Current.PickPhotoAsync();
                    }
                    catch (Exception e)
                    {
                        photoPicked = null;
                        HandleError.Process("ImageManager", "PickPhoto", e, false);
                    }
                }
                if (photoPicked != null)
                {
                    string imgPathDirectory = ImagePathManager.GetPicturesDirectory();
                    //используем метод создания превью для того, чтобы сделать основное фото из оригинального, но с уменьшенным качеством

                    ImagePreviewManager resizedOriginal = new ImagePreviewManager();
                    resizedOriginal.PreviewQualityType = ImageQualityType.Q0x0x40;
                    FileInfo originalFileInfo = new FileInfo(photoPicked.Path);

                    if (resizedOriginal.CreateImagePreview(originalFileInfo.DirectoryName, originalFileInfo.Name, imgPathDirectory, ImagePathManager.GetMediaFilename(mediaId, MediaObjectTypeEnum.Image, false)))
                    {
                        ImagePreviewManager preview = new ImagePreviewManager();
                        preview.PreviewQualityType = _previewImageQuality;
                        if (preview.CreateImagePreview(originalFileInfo.DirectoryName, originalFileInfo.Name, imgPathDirectory, ImagePathManager.GetMediaFilename(mediaId, MediaObjectTypeEnum.Image, true)))
                        {
                            ExifManager exif = new ExifManager();
                            imageGpsCoordinates = exif.GetCoordinates(photoPicked.Path);
                            pickPhotoResult = true;
                        }
                        else
                        {
                            Analytics.TrackEvent("ImageManager: add photo error create preview ", new Dictionary<string, string> { { "mediaId", mediaId } });
                        }
                    }
                    else
                    {
                        Analytics.TrackEvent("ImageManager: error resize photo ", new Dictionary<string, string> { { "mediaId", mediaId } });
                    }
                }
            }

            return (pickPhotoResult, mediaId, imageGpsCoordinates);
        }
        public async Task<(bool result, string newMediaId)> TakePhotoAsync(double latitude, double longitude)
        {
            bool takePhotoResult = false;
            string mediaId = Guid.NewGuid().ToString();
            string photoName = ImagePathManager.GetMediaFilename(mediaId, MediaObjectTypeEnum.Image);

            MediaFile file;
            PermissionManager permissions = new PermissionManager();
            if (await permissions.PermissionGrantedAsync(Plugin.Permissions.Abstractions.Permission.Photos, CommonResource.RoutePoint_RightNeedToTakePhoto))
            {
                try
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Location = new Plugin.Media.Abstractions.Location() { Latitude = latitude, Longitude = longitude, Timestamp = DateTime.Now },
                        Directory = string.Empty,
                        Name = photoName,
                        SaveToAlbum = true,
                        PhotoSize = PhotoSize.Full,
                        CompressionQuality = 40//так же соответствует параметрам качества при выборе фото
                    });
                    takePhotoResult = file != null;
                }
                catch (Exception e)
                {
                    HandleError.Process("ImageManager", "TakePhoto", e, false);
                }
            }

            return (takePhotoResult, mediaId);
        }
    }
}
