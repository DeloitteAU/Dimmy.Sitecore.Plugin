using Dimmy.Cli.Commands.Project.SubCommands;

namespace Dimmy.Sitecore.Plugin
{
    public abstract class SitecoreInitialiseArgument: InitialiseArgument
    {
        public string LicensePath { get; set; }
        
        public string Registry { get; set; }
        
        public string Topology { get; set; }
        
        public string WindowsVersion { get; set; }
    }
}