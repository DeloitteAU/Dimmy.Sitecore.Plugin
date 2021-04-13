using System.Collections.Generic;
using Dimmy.Cli.Commands.Project.SubCommands;
using Dimmy.Engine.Pipelines.InitialiseProject;

namespace Dimmy.Sitecore.Plugin
{
    public class SitecoreInitialiseArgument: InitialiseArgument
    {
        public string LicensePath { get; set; }
        
        public Dictionary<string, string> Registries { get; set; } = new Dictionary<string, string>();
        
        public string Registry { get; set; }
        
        public string Topology { get; set; }
        
        public string WindowsVersion { get; set; }
    }
}