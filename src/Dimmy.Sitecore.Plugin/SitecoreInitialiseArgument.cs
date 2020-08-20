using System.Collections.Generic;
using Dimmy.Engine.Commands.Project;

namespace Dimmy.Sitecore.Plugin
{
    public class SitecoreInitialiseArgument: InitialiseProject
    {
        public string LicensePath { get; set; }
        
        public Dictionary<string, string> Registries { get; set; } = new Dictionary<string, string>();
        
        public string Topology { get; set; }
        
        public string WindowsVersion { get; set; }
    }
}