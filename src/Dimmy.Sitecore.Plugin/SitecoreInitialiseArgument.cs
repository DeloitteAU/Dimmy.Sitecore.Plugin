using System.Collections.Generic;
using Dimmy.Engine.Commands.Project;

namespace Dimmy.Sitecore.Plugin
{
    public class SitecoreInitialiseArgument: InitialiseProject
    {
        public string LicensePath { get; set; }
        
        public Dictionary<string, string> Registries { get; set; }
        
        public string Topology { get; set; }
        
        public string WindowsVersion { get; set; }
    }
}