using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QuestHelper.Managers
{
    public class TrackFileManager
    {
        public IEnumerable<FileInfo> GetTrackFilesFromDirectory()
        {
            IEnumerable<FileInfo> files = new List<FileInfo>();
            DirectoryInfo directory = new DirectoryInfo(ImagePathManager.GetTracksDirectory());
            try
            {
                files =  directory.GetFiles("*")
                    .Where(f => f.Name.EndsWith(".kml") || f.Name.EndsWith(".gpx"));
                
            }
            catch (Exception e)
            {
                HandleError.Process("TrackFileManager", "GetMediaFilesFromDirectory", e, false, directory.FullName);
            }

            return files;
        }

    }
}