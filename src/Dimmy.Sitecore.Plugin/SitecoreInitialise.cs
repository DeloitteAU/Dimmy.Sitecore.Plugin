using System.Collections.Generic;
using System.CommandLine;
using Dimmy.Cli.Commands.Project.SubCommands;
using Dimmy.Engine.Commands;
using Dimmy.Engine.Commands.Project;

namespace Dimmy.Sitecore.Plugin
{
    public class SitecoreInitialise : InitialiseSubCommand
    {
        private readonly IEnumerable<ISitecoreInitialiseVersion> _sitecoreInitialiseCommands;

        public SitecoreInitialise(
            IEnumerable<ISitecoreInitialiseVersion> sitecoreInitialiseCommands,
            ICommandHandler<InitialiseProject> initialiseProjectCommandHandler) : base(initialiseProjectCommandHandler)
        {
            _sitecoreInitialiseCommands = sitecoreInitialiseCommands;
        }

        public override void HydrateCommand(Command command)
        {
            foreach (var sitecoreInitialiseCommand in _sitecoreInitialiseCommands)
            {
                var sc  = new Command(sitecoreInitialiseCommand.Name, sitecoreInitialiseCommand.Description);
                
                var arg = sitecoreInitialiseCommand.HydrateCommand(sc);
                
                //this is added in due to a bug.
                //see https://github.com/DeloitteDigitalAPAC/Dimmy.Sitecore.Plugin/issues/2
                sc.AddOption(new Option<string>("--name"));
                sc.AddOption(new Option<string>("--source-code-path"));
                sc.AddOption(new Option<string>("--working-path"));
                sc.AddOption(new Option<string>("--docker-compose-template"));
                //
                
                sc.AddOption(new Option<string>("--license-path", "Path to the Sitecore License"));
                sc.AddOption(new Option<string>("--registry", $"Defaults to {arg.Registry}"));
                sc.AddOption(new Option<string>("--topology", $"The Sitecore topology. Defaults to {arg.Topology}"));

                command.AddCommand(sc);
            }
        }
        public override string Name => "sitecore";
        public override string Description => "Initialise a Sitecore project.";
    }
}
