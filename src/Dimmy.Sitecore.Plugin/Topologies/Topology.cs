using System.Collections.Generic;

namespace Dimmy.Sitecore.Plugin.Topologies
{
    public abstract class Topology: ITopology
    {
        public abstract string Name { get; }
        public abstract string DockerComposeTemplate { get; }
        public Dictionary<string, string> VariableDictionary { get; }
    }
}