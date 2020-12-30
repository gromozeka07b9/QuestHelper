using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
using FileInfo = System.IO.FileInfo;

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

        public IEnumerable<FileInfo> GetMediaFilesFromDirectory(DirectoryInfo directory)
        {
            IEnumerable<FileInfo> files = new List<FileInfo>();
            try
            {
                files =  directory.GetFiles("*")
                    .Where(f => f.Name.EndsWith(".jpg") || f.Name.EndsWith(".jpeg") || f.Name.EndsWith(".png") ||
                                f.Name.EndsWith(".PNG") || f.Name.EndsWith(".JPEG") || f.Name.EndsWith(".JPG"));
                
            }
            catch (Exception e)
            {
            }

            return files;
        }
    }
}
