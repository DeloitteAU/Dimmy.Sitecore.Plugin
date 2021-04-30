using Dimmy.Cli.Extensions;

namespace Dimmy.Sitecore.Plugin.Versions._10._1._0.Project.SubCommands
{
    public class DeployDevelopmentHelperArgument: SitecoreProjectArgument, IGetProjectArg
    {
        public string ProjectId { get; set; }
        public string WorkingPath { get; set; }
    }
}