using System.Collections.Generic;
using System.CommandLine;
using Dimmy.Engine.Commands;
using Dimmy.Engine.Commands.Project;

namespace Dimmy.Sitecore.Plugin.Versions._9._3._0
{
    public class SitecoreInitialise :SitecoreInitialiseBase<SitecoreInitialiseArgument>
    {
        public override string Name => "sitecore-9.3.0";
        public override string Description => "Initialise a Sitecore 9.3.0 project.";
        
        protected override string Version => "9.3.0";
        
        public SitecoreInitialise(ICommandHandler<InitialiseProject> initialiseProjectCommandHandler) : base(initialiseProjectCommandHandler)
        {
        }
        protected override void DoHydrateCommand(Command command, SitecoreInitialiseArgument arg)
        {
        }
    }
}