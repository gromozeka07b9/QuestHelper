namespace Lighter.Components.Common
{
    public class Root
    {
        public string LighterVersion { get; set; }
        public ComponentInfo Info { get; set; }

        public class ComponentInfo
        {
            public string Type { get; set; }
            public string Version { get; set; }
            public string Description { get; set; }
        }
    }
}