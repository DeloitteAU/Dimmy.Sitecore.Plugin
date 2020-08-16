using System.Collections.Generic;
using System.CommandLine;
using Dimmy.Cli.Commands.Project;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0.Project
{
    public class Sitecore: ProjectSubCommand<SitecoreArgument>
    {
        private readonly IEnumerable<ISitecoreSubCommand> _sitecoreProjectSubCommands;

        public Sitecore(IEnumerable<ISitecoreSubCommand> sitecoreProjectSubCommands)
        {
            _sitecoreProjectSubCommands = sitecoreProjectSubCommands;
        }
        
       
        public override void CommandAction(SitecoreArgument arg)
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