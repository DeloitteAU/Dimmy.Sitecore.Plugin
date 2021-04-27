using System.Linq;
using Dimmy.Engine.Pipelines;
using Dimmy.Engine.Pipelines.StartProject;

namespace Dimmy.Sitecore.Plugin.Pipeline.StartProject.Nodes
{
    public abstract class SitecoreStartNode: Node<IStartProjectContext>
    {
        protected static string GetHostNameFromEnvironmentalVariables(string hostVariable, IStartProjectContext input)
        {
            var hostNameKey = hostVariable
                .Replace("Host(`", "")
                .Replace("`)", "");

            hostNameKey = hostNameKey.Replace("${", string.Empty);
            hostNameKey = hostNameKey.Replace("}", string.Empty);

            var hostName = input.EnvironmentalVariables.Single(s => s.Key == $"{hostNameKey}");
            return hostName.Value;
        }
    }
}