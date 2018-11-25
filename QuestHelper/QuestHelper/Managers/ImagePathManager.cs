using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace QuestHelper.Managers
{
    public static class ImagePathManager
    {
        public static string GetImagePath(string mediaId, bool isPreview = false)
        {
            string filename = isPreview?$"img_{mediaId}_preview.jpg": $"img_{mediaId}.jpg";
            return Path.Combine(DependencyService.Get<IPathService>().PrivateExternalFolder, "pictures", filename);
        }

        public static string GetImageFilename(string mediaId, bool isPreview = false)
        {
            return isPreview ? $"img_{mediaId}_preview.jpg" : $"img_{mediaId}.jpg";
        }
    }
}
