using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuestHelper.Server.Models
{
    public class Base64Manager
    {
        public static void SaveBase64ToFile(string imgBase64, string fileFullPath)
        {
            var bytes = Convert.FromBase64String(imgBase64);
            try
            {
                File.WriteAllBytes(fileFullPath, bytes);
            }
            catch (Exception e)
            {
                throw new Exception($"Error writing route cover: {fileFullPath}", e);
            }
        }
    }
}
