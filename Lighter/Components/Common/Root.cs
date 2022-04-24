namespace Lighter.Components.Common
{
    public abstract class Root
    {
        public string LighterVersion { get; set; }
        public ComponentInfo Info { get; set; }

        public abstract class ComponentInfo
        {
            public string Type { get; set; }
            public string Version { get; set; }
            public string Description { get; set; }
        }
    }
}