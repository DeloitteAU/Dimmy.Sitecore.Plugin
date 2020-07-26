using System.Collections.Generic;

namespace Dimmy.Sitecore.Plugin.Topologies
{
    public interface ITopology
    {
        string Name { get; }
        string DockerComposeTemplate { get; }
        string DockerComposeTemplateName { get; }
        Dictionary<string, string> VariableDictionary { get; }
    }
}
