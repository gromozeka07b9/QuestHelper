using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuestHelper.LocalDB.Model;

namespace QuestHelper.Managers
{
    public class MediaFileManager
    {
        public void Delete(string mediaId, MediaObjectTypeEnum mediaType)
        {
            string mediaPath = ImagePathManager.GetMediaFilename(mediaId, mediaType, false);
            string mediaPreviewPath = ImagePathManager.GetMediaFilename(mediaId, mediaType, true);
            string pictureDir = ImagePathManager.GetPicturesDirectory();
            try
            {
                string fileToDelete = Path.Combine(pictureDir, mediaPath);
                if (File.Exists(fileToDelete))
                {
                    File.Delete(fileToDelete);
                }
            }
            catch (Exception)
            {
            }
            try
            {
                string fileToDelete = Path.Combine(pictureDir, mediaPreviewPath);
                if (File.Exists(fileToDelete))
                {
                    File.Delete(fileToDelete);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
