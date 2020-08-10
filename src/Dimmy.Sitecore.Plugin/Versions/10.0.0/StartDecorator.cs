using System;
using System.CommandLine;
using Dimmy.Cli.Commands.Project;
using Dimmy.Cli.Commands.Project.SubCommands;

namespace Dimmy.Sitecore.Plugin.Versions._10._0._0
{
    public class StartDecorator : ProjectSubCommand<StartArgument>
    {
        private readonly IProjectSubCommand _decorated;

        public StartDecorator(IProjectSubCommand decorated)
        {
            _decorated = decorated;
        }
        
        public override Command GetCommand()
        {
            return _decorated.GetCommand();
        }

        protected override void CommandAction(StartArgument arg)
        {
            Console.WriteLine("HELLO FROM StartDecorator");
            
            _decorated.CommandAction(arg);
        }
    }
}