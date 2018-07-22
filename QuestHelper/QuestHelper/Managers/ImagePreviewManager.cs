using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestHelper.Managers
{
    public class ImagePreviewManager
    {
        public byte[] GetPreviewImage(IMediaService mediaService, byte[] byteArrayOriginal, int width, int height)
        {
            var byteArrayPreview = mediaService.ResizeImage(byteArrayOriginal, width, height);
            /*string previewFilePath = System.IO.Path.Combine("/storage/emulated/0/Android/data/com.sd.QuestHelper/files/Pictures/Photos", "preview.png");
            using (FileStream fs = new FileStream(previewFilePath, FileMode.CreateNew))
            {
                fs.Write(byteArrayPreview, 0, byteArrayPreview.Length);
            }*/
            return byteArrayPreview;
        }
    }
}
