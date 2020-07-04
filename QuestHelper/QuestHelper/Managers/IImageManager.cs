using System.Threading.Tasks;

namespace QuestHelper.Managers
{
    public interface IImageManager
    {
        (bool getMetadataPhotoResult, string newMediaId, Model.GpsCoordinates imageGpsCoordinates) GetPhoto(string photoFullPath);
        Task<(bool pickPhotoResult, string newMediaId, Model.GpsCoordinates imageGpsCoordinates)> PickPhotoAsync();
        Task<(bool result, string newMediaId)> TakePhotoAsync(double latitude, double longitude);
    }
}