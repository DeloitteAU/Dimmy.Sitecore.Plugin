using System.Collections.Generic;
using Dimmy.Engine.Pipelines.InitialiseProject;

namespace Dimmy.Sitecore.Plugin
{
    public class SitecoreInitialiseContext: InitialiseProjectContext
    {
        public string LicensePath { get; set; }
        
        public Dictionary<string, string> Registries { get; set; } = new Dictionary<string, string>();
        
        public string Topology { get; set; }
        
        public string WindowsVersion { get; set; }
    }
}