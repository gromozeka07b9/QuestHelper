using System.IO;
using System.Linq;
using System.Reflection;

namespace Lighter
{
    public sealed class ResourceLoader
    {
        private readonly Assembly fromAssembly;

        public ResourceLoader(Assembly fromAssembly)
        {
            this.fromAssembly = fromAssembly;
        }
        public string GetResourceTextFile(string filename)
        {
            var resourceName = fromAssembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(filename)) ?? string.Empty;
            var stream = fromAssembly.GetManifestResourceStream(resourceName);
            if(stream == null)
                return string.Empty;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

    }
}