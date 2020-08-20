using System;
using Dimmy.Cli.Extensions;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0.Project.SubCommands
{
    public class ResetSitecoreProjectAdminPasswordArgument : SitecoreProjectArgument, IGetProjectArg
    {
        public string ResourcesDirectory { get; set; } = @"C:\resources\";
        public string Password { get; set; } = "b";
        public Guid ProjectId { get; set; }
        public string WorkingPath { get; set; }
    }
}