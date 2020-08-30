using System.Threading.Tasks;

namespace QuestHelper.Managers
{
    public interface IImageManager
    {
        (bool getMetadataPhotoResult, Model.GpsCoordinates imageGpsCoordinates) GetPhoto(string mediaId, string photoFullPath, bool IsPreview = true);
        Task<(bool pickPhotoResult, string newMediaId, Model.GpsCoordinates imageGpsCoordinates)> PickPhotoAsync();
        Task<(bool result, string newMediaId)> TakePhotoAsync(double latitude, double longitude);
        void SetPreviewImageQuality(ImageQualityType qualityType);
        ImageQualityType GetPreviewImageQuality();
    }
}