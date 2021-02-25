using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuestHelper.LocalDB.Model;
using FileInfo = System.IO.FileInfo;

namespace QuestHelper.Managers
{
    public class MediaFileManager : IMediaFileManager
    {
        string _pictureDir = ImagePathManager.GetPicturesDirectory();
        public void Delete(string mediaId, MediaObjectTypeEnum mediaType)
        {
            string mediaPath = ImagePathManager.GetMediaFilename(mediaId, mediaType, false);
            string mediaPreviewPath = ImagePathManager.GetMediaFilename(mediaId, mediaType, true);
            try
            {
                string fileToDelete = Path.Combine(_pictureDir, mediaPath);
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
                string fileToDelete = Path.Combine(_pictureDir, mediaPreviewPath);
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
                HandleError.Process("MediaFileManager", "GetMediaFilesFromDirectory", e, false, directory.FullName);
            }

            return files;
        }

        public bool FileExistInMediaCatalog(string imgFilename)
        {
            try
            {
                string fullpath = Path.Combine(_pictureDir, imgFilename);
                return File.Exists(fullpath);
            }
            catch (Exception e)
            {
                HandleError.Process("MediaFileManager", "FileExistInMediaCatalog", e, false, imgFilename);
            }

            return false;
        }

        public bool SaveMediaFileFromBase64(string filename, string contentBase64)
        {
            try
            {
                string fullpath = Path.Combine(_pictureDir, filename);
                var bytes = Convert.FromBase64String(contentBase64);
                using (FileStream fs = new FileStream(fullpath, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                }
                return true;
            }
            catch (Exception e)
            {
                HandleError.Process("MediaFileManager", "SaveMediaFileFromBase64", e, false, filename);
            }

            return false;
        }
    }

    public interface IMediaFileManager
    {
        void Delete(string mediaId, MediaObjectTypeEnum mediaType);
        IEnumerable<FileInfo> GetMediaFilesFromDirectory(DirectoryInfo directory);
        bool FileExistInMediaCatalog(string imgFilename);
        bool SaveMediaFileFromBase64(string filename, string contentBase64);
    }
}
