using System.Collections.Generic;
using System.CommandLine;
using Dimmy.Cli.Commands.Project;

namespace Dimmy.Sitecore.Plugin.Versions._10._1._0.Project
{
    public class SitecoreProject: ProjectSubCommand<SitecoreProjectArgument>
    {
        private readonly IEnumerable<ISitecoreProjectCommand> _sitecoreProjectSubCommands;

        public SitecoreProject(IEnumerable<ISitecoreProjectCommand> sitecoreProjectSubCommands)
        {
            _sitecoreProjectSubCommands = sitecoreProjectSubCommands;
        }
        
       
        public override void CommandAction(SitecoreProjectArgument arg)
        {
            
        }

        public override Command BuildCommand()
        {
            var command = new Command("sitecore-10.0.0", "Command to interaction with Sitecore 10.0.0 ");
            AddSubCommands(command, _sitecoreProjectSubCommands);
            return command;
        }
    }
}