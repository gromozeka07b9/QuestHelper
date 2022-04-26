using System.IO;
using System.Linq;

namespace QuestHelper
{
    public class ResourceLoader
    {
        public static string GetResourceTextFile(string filename)
        {
            var resourceName = GetType().Module.Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(filename)) ?? string.Empty;
            var stream = GetType().Module.Assembly.GetManifestResourceStream(resourceName);
            if(stream == null)
                return string.Empty;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

    }
}