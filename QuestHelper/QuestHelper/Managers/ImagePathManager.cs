using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuestHelper.LocalDB.Model;
using Xamarin.Forms;

namespace QuestHelper.Managers
{
    public static class ImagePathManager
    {
        public static string GetImagePath(string mediaId, LocalDB.Model.MediaObjectTypeEnum mediaType, bool isPreview = false)
        {
            string filename = GetMediaFilename(mediaId, mediaType, isPreview);
            return Path.Combine(DependencyService.Get<IPathService>().PrivateExternalFolder, "pictures", filename);
        }

        public static string GetMediaFilename(string mediaId, LocalDB.Model.MediaObjectTypeEnum mediaType, bool isPreview = false)
        {
            string filename = string.Empty;
            if (mediaType == MediaObjectTypeEnum.Image)
            {
                filename = isPreview ? $"img_{mediaId}_preview.jpg" : $"img_{mediaId}.jpg";
            }
            else if (mediaType == MediaObjectTypeEnum.Audio)
            {
                filename = $"audio_{mediaId}.3gp";
            }
            else
            {
                throw new Exception("Unknown media type!");
            }
            return filename;
        }

        public static string GetPicturesDirectory()
        {
            return Path.Combine(DependencyService.Get<IPathService>().PrivateExternalFolder, "pictures");
        }
        public static string GetTracksDirectory()
        {
            return Path.Combine(DependencyService.Get<IPathService>().PrivateExternalFolder, "tracks");
        }
    }
}
