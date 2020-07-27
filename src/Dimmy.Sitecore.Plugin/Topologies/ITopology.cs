using System.Collections.Generic;

namespace Dimmy.Sitecore.Plugin.Topologies
{
    public interface ITopology
    {
        string Name { get; }
        string DockerComposeTemplate { get; }
        Dictionary<string, string> VariableDictionary { get; }
    }
}
